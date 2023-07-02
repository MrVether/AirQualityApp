using Microsoft.Extensions.Caching.Memory;
using AirQualityApp.Models;
using System;
using AirQualityApp.Interfaces.Cache;
using AirQualityApp.Interfaces.AirQualityAPI;

namespace AirQualityApp.Services.Cache
{
    public class DetailedMeasurmentCacheService : IDetailedMeasurementCacheService
    {
        private IMemoryCache _cache;

        public DetailedMeasurmentCacheService(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        public List<Measurement> GetDetailedMeasurementsAsync()
        {
            return _cache.Get<List<Measurement>>("CachedDetailedMeasurements");
        }

        public void Update(List<Measurement> measurements)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(1)); // set cache to expire after 1 hour

            _cache.Set("CachedDetailedMeasurements", measurements, cacheEntryOptions);
        }

        public async void UpdateDataInCache(IAirQualityDataStorage airQualityDatabaseStorage)
        {
            var measurments = await airQualityDatabaseStorage.FetchMeasurementsFromDb();
            Update(measurments);
        }
    }
}
