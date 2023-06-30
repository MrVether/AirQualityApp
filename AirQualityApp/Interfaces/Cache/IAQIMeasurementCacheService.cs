using AirQualityApp.Interfaces.AirQualityAPI;
using AirQualityApp.Models;
using System.Collections.Generic;

namespace AirQualityApp.Interfaces.Cache
{
    public interface IAQIMeasurementCacheService
    {
        List<Marker> GetCachedMeasurements();
        void Update(List<Marker> measurements);
        void UpdateDataInCache(IAirQualityDataStorage airQualityDatabaseStorage);
    }
}