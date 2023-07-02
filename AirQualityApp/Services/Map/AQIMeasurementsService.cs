using AirQualityApp.Interfaces;
using AirQualityApp.Interfaces.AirQualityAPI;
using AirQualityApp.Interfaces.Cache;
using AirQualityApp.Interfaces.Map;
using AirQualityApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirQualityApp.Services.Map
{
    public class AQIMeasurementsService : IAQIMeasurementsService
    {
        private readonly IAQIMeasurementCacheService _aqiMeasurementCacheService;
        private readonly IDetailedMeasurementCacheService _detailedMeasurementCacheService;
        private readonly IAirQualityDataStorage _airQualityDataStorage;

        public AQIMeasurementsService(IAQIMeasurementCacheService aqiMeasurementCacheService,
                                      IDetailedMeasurementCacheService detailedMeasurementCacheService,
                                      IAirQualityDataStorage airQualityDataStorage)
        {
            _aqiMeasurementCacheService = aqiMeasurementCacheService;
            _detailedMeasurementCacheService = detailedMeasurementCacheService;
            _airQualityDataStorage = airQualityDataStorage;
        }

        public async Task<List<Marker>> GetAQIMeasurementsAsync()
        {
            var measurements = _aqiMeasurementCacheService.GetCachedMeasurements();

            if (measurements == null)
            {
                measurements = await _airQualityDataStorage.FetchAQIMeasurementsFromDb();
                _aqiMeasurementCacheService.Update(measurements);
            }

            return measurements;
        }

        public async Task<List<Measurement>> GetDetailedMeasurementsAsync()
        {
            var measurements = _detailedMeasurementCacheService.GetDetailedMeasurementsAsync();

            if (measurements == null)
            {
                measurements = await _airQualityDataStorage.FetchMeasurementsFromDb();
                _detailedMeasurementCacheService.Update(measurements);
            }

            return measurements;
        }
    }
}
