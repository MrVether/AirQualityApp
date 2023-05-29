using AirQualityApp.BackgroundServices;
using AirQualityApp.Hubs;
using AirQualityApp.Models;
using AirQualityApp.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddScoped<OpenAqApiClient>();
builder.Services.AddHostedService<DataRefreshService>();
builder.Services.AddSignalR();
builder.Services.AddSingleton<MeasurementCacheService>();



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
app.MapHub<MapHub>("/mapHub");

app.Run();
