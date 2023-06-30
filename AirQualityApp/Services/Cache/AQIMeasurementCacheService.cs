

using Microsoft.Extensions.Caching.Memory;
using AirQualityApp.Models;
using System;
using AirQualityApp.Interfaces.Cache;
using System.Collections.Generic;
using AirQualityApp.Interfaces.AirQualityAPI;

namespace AirQualityApp.Services.Cache
{
    public class AQIMeasurementCacheService : IAQIMeasurementCacheService
    {
        private readonly IMemoryCache _cache;

        public AQIMeasurementCacheService(IMemoryCache memoryCache)
        {
            _cache = memoryCache;
        }

        public List<Marker> GetCachedMeasurements()
        {
            return _cache.Get<List<Marker>>("CachedMeasurements");
        }

        public void Update(List<Marker> measurements)
        {
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromHours(1)); // set cache to expire after 1 hour

            _cache.Set("CachedMeasurements", measurements, cacheEntryOptions);
        }

        public async void UpdateDataInCache(IAirQualityDataStorage airQualityDatabaseStorage)
        {
            var measurements = await airQualityDatabaseStorage.FetchAQIMeasurementsFromDb();
            Update(measurements);
        }
    }
}
