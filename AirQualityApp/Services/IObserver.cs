using AirQualityApp.Models;

namespace AirQualityApp.Services
{
    public interface IObserver
    {
        void Update(List<Measurement> measurements);
    }

}
