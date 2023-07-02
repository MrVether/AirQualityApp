using AirQualityApp.Interfaces;
using AirQualityApp.Interfaces.AirQualityAPI;
using AirQualityApp.Interfaces.Cache;
using AirQualityApp.Interfaces.Helpers;
using AirQualityApp.Models;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AirQualityApp.Services.Helpers
{
    public class DotNetInteropHelper : IDotNetInteropHelper
    {
        private readonly IDetailedMeasurementCacheService _detailedCacheService;
        private readonly IAirQualityDataStorage _airQualityDataStorage;

        public DotNetInteropHelper(IDetailedMeasurementCacheService detailedCacheService, IAirQualityDataStorage airQualityDataStorage)
        {
            _detailedCacheService = detailedCacheService;
            _airQualityDataStorage = airQualityDataStorage;
        }

        [JSInvokable("GetDetailedMeasurementsAsync")]
        public async Task<List<Measurement>> GetMeasurementsAsync(string location)
        {
            var measurements = _detailedCacheService.GetDetailedMeasurementsAsync();

            if (measurements == null)
            {
                measurements = await _airQualityDataStorage.FetchMeasurementsFromDb();
                _detailedCacheService.Update(measurements);
            }
            var filteredMeasurements = measurements.Where(m => m.Location == location).ToList();

            return filteredMeasurements;
        }
    }
}
