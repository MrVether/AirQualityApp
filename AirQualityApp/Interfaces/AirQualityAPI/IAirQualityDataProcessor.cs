using AirQualityApp.Models;

namespace AirQualityApp.Interfaces.AirQualityAPI
{
    public interface IAirQualityDataProcessor
    {
        Dictionary<string, Dictionary<string, Measurement>> GroupMeasurementsByLocation(List<Measurement> measurements);
        Dictionary<string, Dictionary<string, Marker>> GroupMarkersByLocation(List<Marker> markers);
        List<Marker> CreateMarkersFromMeasurements(List<Marker> measurements);
    }
}
