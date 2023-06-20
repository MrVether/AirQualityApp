using Microsoft.Extensions.Caching.Memory;
using AirQualityApp.Models;
using System;

namespace AirQualityApp.Services
{
    public class MeasurementCacheService
    {
        private IMemoryCache _cache;

        public MeasurementCacheService(IMemoryCache memoryCache)
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

        public async void UpdateDataInCache(OpenAqApiClient openAqApiClient)
        {
            var measurments = await openAqApiClient.FetchAQIMeasurementsFromDb();
            Update(measurments);
        }
    }
}
