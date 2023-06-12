using AirQualityApp.Models;

namespace AirQualityApp.Services
{
    public class MeasurementCacheService : IObserver
    {
        private List<Measurement> cachedMeasurements;

        public List<Measurement> CachedMeasurements
        {
            get { return cachedMeasurements; }
            set { cachedMeasurements = value; }
        }

        public void Update(List<Measurement> measurements)
        {
            CachedMeasurements = measurements;
        }
    }

}
