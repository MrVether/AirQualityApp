using AirQualityApp.Services;
using System.Diagnostics;

namespace AirQualityApp.BackgroundServices
{
    public class DataRefreshService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public DataRefreshService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }



        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var openAqApiClient = scope.ServiceProvider.GetRequiredService<OpenAqApiClient>();

                    var measurementCacheService = scope.ServiceProvider.GetRequiredService<MeasurementCacheService>();
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
                            stopwatch.Reset();
                            stopwatch.Start();
                            Console.WriteLine("Updating no2 for: " + country.Name + " Country Code: " + country.Code);
                            var no2Measurements = await openAqApiClient.GetGlobalMeasurementsAsync("no2", country.Code);
                            stopwatch.Stop();
                            Console.WriteLine("Time taken for NO2: " + stopwatch.Elapsed.TotalSeconds + " seconds.");
                            stopwatch.Reset();
                            stopwatch.Start();
                            Console.WriteLine("Updating o3 for: " + country.Name + " Country Code: " + country.Code);
                            var o3Measurements = await openAqApiClient.GetGlobalMeasurementsAsync("o3", country.Code);
                            stopwatch.Stop();
                            Console.WriteLine("Time taken for O3: " + stopwatch.Elapsed.TotalSeconds + " seconds.");
                            stopwatch.Reset();
                            stopwatch.Start();
                            Console.WriteLine("Updating so2 for: " + country.Name + " Country Code: " + country.Code);
                            var so2Measurements = await openAqApiClient.GetGlobalMeasurementsAsync("so2", country.Code);
                            stopwatch.Stop();
                            Console.WriteLine("Time taken for SO2: " + stopwatch.Elapsed.TotalSeconds + " seconds.");
                            stopwatch.Reset();
                            stopwatch.Start();
                            Console.WriteLine("Updating co for: " + country.Name + " Country Code: " + country.Code);
                            var coMeasurements = await openAqApiClient.GetGlobalMeasurementsAsync("co", country.Code);
                            stopwatch.Stop();
                            Console.WriteLine("Time taken for CO: " + stopwatch.Elapsed.TotalSeconds + " seconds.");
                            stopwatch.Reset();
                            stopwatch.Start();
                            Console.WriteLine("Updating bc for: " + country.Name + " Country Code: " + country.Code);
                            var bcMeasurements = await openAqApiClient.GetGlobalMeasurementsAsync("bc", country.Code);
                            stopwatch.Stop();
                            Console.WriteLine("Time taken for BC: " + stopwatch.Elapsed.TotalSeconds + " seconds.");
                            stopwatch.Reset();
                            stopwatch.Start();
                            Console.WriteLine("Updating ch4 for: " + country.Name + " Country Code: " + country.Code);
                            var ch4Measurements = await openAqApiClient.GetGlobalMeasurementsAsync("ch4", country.Code);
                            stopwatch.Stop();
                            Console.WriteLine("Time taken for CH4: " + stopwatch.Elapsed.TotalSeconds + " seconds.");
                            stopwatch.Reset();
                            stopwatch.Start();
                            Console.WriteLine("Updating no for: " + country.Name + " Country Code: " + country.Code);
                            var noMeasurements = await openAqApiClient.GetGlobalMeasurementsAsync("no", country.Code);
                            stopwatch.Stop();
                            Console.WriteLine("Time taken for NO: " + stopwatch.Elapsed.TotalSeconds + " seconds.");
                            stopwatch.Reset();
                            stopwatch.Start();
                            Console.WriteLine("Updating nox for: " + country.Name + " Country Code: " + country.Code);
                            var noxMeasurements = await openAqApiClient.GetGlobalMeasurementsAsync("nox", country.Code);
                            stopwatch.Stop();
                            Console.WriteLine("Time taken for NOX: " + stopwatch.Elapsed.TotalSeconds + " seconds.");
                            stopwatch.Reset();
                            stopwatch.Start();
                            Console.WriteLine("Updating co2 for: " + country.Name + " Country Code: " + country.Code);
                            var co2Measurements = await openAqApiClient.GetGlobalMeasurementsAsync("co2", country.Code);
                            stopwatch.Stop();
                            Console.WriteLine("Time taken for CO2: " + stopwatch.Elapsed.TotalSeconds + " seconds.");
                            stopwatch.Reset();
                            stopwatch.Start();
                            Console.WriteLine("Updating pm1 for: " + country.Name + " Country Code: " + country.Code);
                            var pm1Measurements = await openAqApiClient.GetGlobalMeasurementsAsync("pm1", country.Code);
                            stopwatch.Stop();
                            Console.WriteLine("Time taken for PM1: " + stopwatch.Elapsed.TotalSeconds + " seconds.");
                            stopwatch.Reset();
                            stopwatch.Start();
                            Console.WriteLine("Updating pm4 for: " + country.Name + " Country Code: " + country.Code);
                            var pm4Measurements = await openAqApiClient.GetGlobalMeasurementsAsync("pm4", country.Code);
                            stopwatch.Stop();
                            Console.WriteLine("Time taken for PM4: " + stopwatch.Elapsed.TotalSeconds + " seconds.");
                            stopwatch.Reset();


                            var measurements = pm10Measurements.Concat(pm25Measurements).ToList();

                            var measurementsByLocation = openAqApiClient.GroupMeasurementsByLocation(measurements);


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
                    measurementCacheService.UpdateDataInCache(openAqApiClient);

                    Console.WriteLine("Updated: " + DateTime.Now);
                    await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
                }
            }
        }
    }
}
