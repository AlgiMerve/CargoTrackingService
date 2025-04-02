using RabbitMQ.Client;
using Microsoft.AspNetCore.Mvc;
using DocumentApi.Data;
using DocumentApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocumentApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DocumentsController : ControllerBase
    {
        private readonly DocumentContext _context;
        private readonly ConnectionFactory _factory;

        public DocumentsController(DocumentContext context, IConfiguration configuration)
        {
            _context = context;
            _factory = new ConnectionFactory() { HostName = configuration["RabbitMQ:HostName"] };
        }

        [HttpGet]
        public IEnumerable<Document> Get()
        {
            return _context.Documents.ToList();
        }

        [HttpGet("{id}")]
        public Document? Get(int id)
        {
            return _context.Documents.Find(id);
        }

        [HttpPost]
        public void Post([FromBody] Document? document)
        {
            if (document != null)
            {
                _context.Documents.Add(document);
                _context.SaveChanges();

                if (!string.IsNullOrEmpty(document.PTTTrackingNumber))
                {
                    using (var connection = _factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {
                        channel.QueueDeclare(queue: "ptt_tracking_numbers",
                                             durable: false,
                                             exclusive: false,
                                             autoDelete: false,
                                             arguments: null);

                        string message = document.PTTTrackingNumber;
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish(exchange: "",
                                             routingKey: "ptt_tracking_numbers",
                                             basicProperties: null,
                                             body: body);
                    }
                }
            }
        }

        [HttpPut("{id}")]
        public void Put(int id, [FromBody] Document document)
        {
            var existingDocument = _context.Documents.Find(id);
            if (existingDocument != null)
            {
                existingDocument.Name = document.Name;
                existingDocument.FilePath = document.FilePath;
                existingDocument.FileType = document.FileType;
                existingDocument.PTTTrackingNumber = document.PTTTrackingNumber;
                existingDocument.PTTCargoStatus = document.PTTCargoStatus;
                _context.SaveChanges();
            }
        }

        [HttpDelete("{id}")]
        public void Delete(int id)
        {
            var document = _context.Documents.Find(id);
            if (document != null)
            {
                _context.Documents.Remove(document);
                _context.SaveChanges();
            }
        }

        [HttpPut("UpdateCargoStatus")]
        public IActionResult UpdateCargoStatus(string trackingNumber, string cargoStatus)
        {
          var document = _context.Documents.FirstOrDefault(d => d.PTTTrackingNumber == trackingNumber);
          if (document != null)
           {
             document.PTTCargoStatus = cargoStatus;
             _context.SaveChanges();
             return Ok("Cargo status updated.");
            }
          return NotFound("Document not found.");
        }
    }
}