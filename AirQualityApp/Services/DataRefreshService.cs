﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AirQualityApp.Services;
using AirQualityApp.Hubs;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace AirQualityApp.BackgroundServices
{
    public class DataRefreshService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IHubContext<MapHub> _hubContext;


        public DataRefreshService(IServiceScopeFactory serviceScopeFactory, IHubContext<MapHub> hubContext)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _hubContext = hubContext;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {


            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var openAqApiClient = scope.ServiceProvider.GetRequiredService<OpenAqApiClient>();
                    var countries = openAqApiClient.GetCountriesFromDb();
                    Console.WriteLine("Updating..");
                    foreach (var country in countries)
                    {
      

                        try
                        {

                            var pm10Measurements =
                                await openAqApiClient.GetGlobalMeasurementsAsync("pm10", country.Code);
                            var pm25Measurements =
                                await openAqApiClient.GetGlobalMeasurementsAsync("pm25", country.Code);

                            var measurementsByLocation =
                                openAqApiClient.GroupMeasurementsByLocation(pm10Measurements.Concat(pm25Measurements)
                                    .ToList());

                            await _hubContext.Clients.All.SendAsync("ReceiveMeasurements", measurementsByLocation);
                        }
                        catch (System.Net.Sockets.SocketException socketException)
                        {
                            if (socketException.SocketErrorCode == System.Net.Sockets.SocketError.HostNotFound)
                            {
                                break;
                            }

                            throw;
                        }
                    }
                    Console.WriteLine("Updated: " + DateTime.Now);
                    await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
                }
            }
        }

    }
}
