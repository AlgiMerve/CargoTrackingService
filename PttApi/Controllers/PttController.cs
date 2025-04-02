using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using Hangfire;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;

namespace PttApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PttController : ControllerBase
    {
        private readonly ConnectionFactory _factory;
        private readonly HttpClient _httpClient;
        private readonly IBackgroundJobClient _backgroundJobs;
        private readonly IConfiguration _configuration;

        public PttController(IConfiguration configuration, HttpClient httpClient, IBackgroundJobClient backgroundJobs)
        {
            _factory = new ConnectionFactory() { HostName = configuration["RabbitMQ:HostName"] };
            _httpClient = httpClient;
            _backgroundJobs = backgroundJobs;
            _configuration = configuration;
        }

        [HttpGet("CheckCargoStatus")]
        public IActionResult CheckCargoStatus()
        {
            _backgroundJobs.Enqueue(() => CheckCargoStatusJob());
            return Ok("Cargo status check job enqueued.");
        }

        public async Task CheckCargoStatusJob()
        {
            using (var connection = _factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "ptt_tracking_numbers",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    var cargoStatus = await GetCargoStatusFromPtt(message);

                    await UpdateCargoStatusInDocumentApi(message, cargoStatus);
                };

                channel.BasicConsume(queue: "ptt_tracking_numbers",
                                     autoAck: true,
                                     consumer: consumer);

                await Task.Delay(Timeout.Infinite);
            }
        }

        private async Task<string> GetCargoStatusFromPtt(string trackingNumber)
        {
            try
            {
                var url = $"https://gonderitakip.ptt.gov.tr/Takip/{trackingNumber}";
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var html = await response.Content.ReadAsStringAsync();
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(html);

                var cargoStatusNode = htmlDoc.DocumentNode.SelectSingleNode("//*[@id=\"durum\"]/strong");
                if (cargoStatusNode != null)
                {
                    return cargoStatusNode.InnerText;
                }

                return "Kargo durumu bulunamadı.";
            }
            catch (Exception ex)
            {
                return $"Hata: {ex.Message}";
            }
        }

        private async Task UpdateCargoStatusInDocumentApi(string trackingNumber, string cargoStatus)
        {
            try
            {
                var documentApiUrl = _configuration["DocumentApiUrl"];
                var apiUrl = $"{documentApiUrl}/Documents/UpdateCargoStatus?trackingNumber={trackingNumber}&cargoStatus={cargoStatus}";
                var response = await _httpClient.PutAsync(apiUrl, null);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                // Hata yönetimi
            }
        }
    }
}