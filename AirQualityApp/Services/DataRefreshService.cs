using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AirQualityApp.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using AirQualityApp.Models;

namespace AirQualityApp.BackgroundServices
{
    public class DataRefreshService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly List<IObserver> observers;
        public bool CanStart { get; set;}=false;

        public DataRefreshService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
            observers = new List<IObserver>();
        }
        
        public void Attach(IObserver observer)
        {
            observers.Add(observer);
        }

        public void Detach(IObserver observer)
        {
            observers.Remove(observer);
        }

        public void Notify(List<Measurement> measurements)
        {
            foreach (IObserver observer in observers)
            {
                observer.Update(measurements);
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            if (CanStart) {
                while (!stoppingToken.IsCancellationRequested)
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var openAqApiClient = scope.ServiceProvider.GetRequiredService<OpenAqApiClient>();

                        var measurementCacheService = scope.ServiceProvider.GetRequiredService<MeasurementCacheService>();
                        openAqApiClient.Attach(measurementCacheService);

                        var countries = await openAqApiClient.GetCountriesAsync();
                        Console.WriteLine("Updating..");
                        foreach (var country in countries)
                        {
                            try
                            {
                                Stopwatch stopwatch = new Stopwatch();

                                stopwatch.Start();
                                Console.WriteLine("Updating pm10 for: " + country.Name + " Country Code: " + country.Code);
                                var pm10Measurements = await openAqApiClient.GetGlobalMeasurementsAsync("pm10", country.Code);
                                stopwatch.Stop();
                                Console.WriteLine("Time taken for PM10: " + stopwatch.Elapsed.TotalSeconds + " seconds.");

                                stopwatch.Reset();

                                stopwatch.Start();
                                Console.WriteLine("Updating pm2.5 for: " + country.Name + " Country Code: " + country.Code);

                                var pm25Measurements = await openAqApiClient.GetGlobalMeasurementsAsync("pm25", country.Code);
                                stopwatch.Stop();
                                Console.WriteLine("Time taken for PM2.5: " + stopwatch.Elapsed.TotalSeconds + " seconds.");

                                var measurements = pm10Measurements.Concat(pm25Measurements).ToList();

                                var measurementsByLocation = openAqApiClient.GroupMeasurementsByLocation(measurements);

                                Notify(measurements);

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
                        await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
                    }
                }
            } else {
                     await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                CanStart = true;
                                                   await ExecuteAsync(stoppingToken);}
       
        }
    }
}
