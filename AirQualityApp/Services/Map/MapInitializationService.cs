using AirQualityApp.Interfaces;
using AirQualityApp.Interfaces.AirQualityAPI;
using AirQualityApp.Interfaces.Cache;
using AirQualityApp.Interfaces.Map;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace AirQualityApp.Services.Map
{
    public class MapInitializationService : IMapInitializationService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly IAQIMeasurementsService _aqiMeasurementsService;
        private readonly IAirQualityDataProcessor _airQualityDataProcessor;

        public MapInitializationService(IJSRuntime jsRuntime, IAQIMeasurementsService aqiMeasurementsService, IAirQualityDataProcessor airQualityDataProcessor)
        {
            _jsRuntime = jsRuntime;
            _aqiMeasurementsService = aqiMeasurementsService;
            _airQualityDataProcessor = airQualityDataProcessor;
        }

        public async Task<IJSObjectReference> InitializeMapAsync()
        {
            var map = await _jsRuntime.InvokeAsync<IJSObjectReference>("initializeAirQualityMap");

            Console.WriteLine("Map Initialized");
            Console.WriteLine("Fetching measurements from DB");
            var aqiMeasurements = await _aqiMeasurementsService.GetAQIMeasurementsAsync();
            var dataForMarkers = _airQualityDataProcessor.CreateMarkersFromMeasurements(aqiMeasurements);
            Console.WriteLine("Measurements fetched from DB");
            Console.WriteLine("Fetching detailed measurements from DB");
            await _aqiMeasurementsService.GetDetailedMeasurementsAsync();
            Console.WriteLine("Detailed measurements fetched from DB");

            try
            {
                Console.WriteLine("Grouping measurements by location");
                var markersByLocation = _airQualityDataProcessor.GroupMarkersByLocation(dataForMarkers);
                Console.WriteLine("MarkersByLocation: " + markersByLocation);

                await _jsRuntime.InvokeVoidAsync("updateMarkers", map, markersByLocation);

                Console.WriteLine("Markers Added");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            return map;
        }
    }
}
