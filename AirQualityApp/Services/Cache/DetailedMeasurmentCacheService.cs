using Microsoft.Extensions.Caching.Memory;
using AirQualityApp.Models;
using System;
using AirQualityApp.Interfaces.Cache;

namespace AirQualityApp.Services.Cache
{
    public class DetailedMeasurmentCacheService : IDetailedMeasurementCacheService
    {
        private IMemoryCache _cache;

        public DetailedMeasurmentCacheService(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        public List<Measurement> GetCachedMeasurements()
        {
            return _cache.Get<List<Measurement>>("CachedMeasurements");
        }

        public void Update(List<Measurement> measurements)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(1)); // set cache to expire after 1 hour

            _cache.Set("CachedMeasurements", measurements, cacheEntryOptions);
        }

        public async void UpdateDataInCache(AirQualityDatabaseDataStorage airQualityDatabaseStorage)
        {
            var measurments = await airQualityDatabaseStorage.FetchMeasurementsFromDb();
            Update(measurments);
        }
    }
}
