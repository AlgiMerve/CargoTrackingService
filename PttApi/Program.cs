using Hangfire;
using Hangfire.MemoryStorage;
using PttApi.Controllers;
using Microsoft.Extensions.Configuration;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Hangfire Yapılandırması
builder.Services.AddHangfire(configuration => configuration
    .UseMemoryStorage());

builder.Services.AddHangfireServer();

// HttpClient Yapılandırması
builder.Services.AddHttpClient();

// IConfiguration Yapılandırması
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Hangfire Dashboard'u ekle
app.UseHangfireDashboard();

// Hangfire Dashboard'u erişilebilir hale getir
app.MapHangfireDashboard();

// Tekrarlayan işi başlat
RecurringJob.AddOrUpdate<PttController>("check-cargo-status", controller => controller.CheckCargoStatusJob(), Cron.Hourly);

app.Run();