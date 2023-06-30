using AirQualityApp.Models;

namespace AirQualityApp.Interfaces.Cache
{
    public interface IDetailedMeasurementCacheService
    {
        List<Measurement> GetCachedMeasurements();
        void Update(List<Measurement> measurements);
        void UpdateDataInCache(AirQualityDatabaseDataStorage airQualityDatabaseStorage);
    }
}
