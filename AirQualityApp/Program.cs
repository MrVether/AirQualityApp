using AirQualityApp.Services.BackgroundServices;
using AirQualityApp.Models;
using AirQualityApp.Services;
using AirQualityApp.Services.Cache;
using Microsoft.EntityFrameworkCore;
using AirQualityApp.Interfaces.AirQualityAPI;
using AirQualityApp.Services.AirQuaityAPI;
using AirQualityApp.Interfaces.Cache;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IAQIMeasurementCacheService, AQIMeasurementCacheService>();
builder.Services.AddSingleton<IDetailedMeasurementCacheService, DetailedMeasurmentCacheService>();

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddSignalR();

builder.Services.AddScoped<IAirQualityDataProvider, AirQualityDataProvider>();
builder.Services.AddScoped<IAirQualityDataStorage, AirQualityDatabaseDataStorage>();
builder.Services.AddScoped<IAirQualityDataProcessor, AirQualityDataProcessor>();





var connectionString = builder.Configuration.GetConnectionString("AirQualityDB");
builder.Services.AddDbContext<AirQualityContext>(options => options.UseSqlServer(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
