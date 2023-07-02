using AirQualityApp.Interfaces.AirQualityAPI;
using AirQualityApp.Models;

namespace AirQualityApp.Interfaces.Cache
{
    public interface IDetailedMeasurementCacheService
    {
        List<Measurement> GetDetailedMeasurementsAsync();
        void Update(List<Measurement> measurements);
        void UpdateDataInCache(IAirQualityDataStorage airQualityDatabaseStorage);
    }
}
