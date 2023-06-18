using AirQualityApp.Services;
using System.Diagnostics;

namespace AirQualityApp.BackgroundServices
{
    public class ScheduledTask
    {
        public string MeasurementType { get; set; }
        public TimeSpan Interval { get; set; }
        public DateTime LastRunTime { get; set; }
    }

    public class DataRefreshService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private List<ScheduledTask> tasks;

        public DataRefreshService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;

            tasks = new List<ScheduledTask>
        {
            new ScheduledTask { MeasurementType = "pm10", Interval = TimeSpan.FromMinutes(15) },
            new ScheduledTask { MeasurementType = "pm25", Interval = TimeSpan.FromHours(1) },
            new ScheduledTask { MeasurementType = "no2", Interval = TimeSpan.FromHours(2) },
            new ScheduledTask { MeasurementType = "o3", Interval = TimeSpan.FromHours(4) },
            new ScheduledTask { MeasurementType = "so2", Interval = TimeSpan.FromHours(6) },
            new ScheduledTask { MeasurementType = "co", Interval = TimeSpan.FromHours(8) },
            new ScheduledTask { MeasurementType = "bc", Interval = TimeSpan.FromHours(12) },
            new ScheduledTask { MeasurementType = "ch4", Interval = TimeSpan.FromDays(1) },
            new ScheduledTask { MeasurementType = "no", Interval = TimeSpan.FromDays(2) },
            new ScheduledTask { MeasurementType = "nox", Interval = TimeSpan.FromDays(3) },
            new ScheduledTask { MeasurementType = "co2", Interval = TimeSpan.FromDays(7) },
            new ScheduledTask { MeasurementType = "pm1", Interval = TimeSpan.FromDays(7) },
            new ScheduledTask { MeasurementType = "pm4", Interval = TimeSpan.FromDays(7) }
        };
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

                    foreach (var task in tasks)
                    {
                        if (DateTime.UtcNow - task.LastRunTime >= task.Interval)
                        {
                            foreach (var country in countries)
                            {
                                Console.WriteLine($"Getting measurements for {country.Name} for {task.MeasurementType}");
                               await openAqApiClient.GetGlobalMeasurementsAsync(task.MeasurementType, country.Code,task.Interval);

                            }

                            task.LastRunTime = DateTime.UtcNow;
                        }
                    }

                    measurementCacheService.UpdateDataInCache(openAqApiClient);
                }

                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }

}

