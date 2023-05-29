using AirQualityApp.Models;

namespace AirQualityApp.Services
{
    public class MeasurementCacheService
    {
        private List<Measurement> cachedMeasurements;

        public List<Measurement> CachedMeasurements
        {
            get { return cachedMeasurements; }
            set { cachedMeasurements = value; }
        }
    }

}
