using AirQualityApp.Models;

namespace AirQualityApp.Interfaces.AirQualityAPI
{
    public interface IAirQualityDataStorage
    {
        List<Country> GetCountriesFromDb();
        List<Measurement> GetGlobalMeasurementsFromDb(string parameter, string country);
        List<Marker> GetAQIMeasurmentFromDB(string parameter, string country);
        Task<List<Measurement>> FetchMeasurementsFromDb();
        Task<List<Marker>> FetchAQIMeasurementsFromDb();
        Task<Measurement> GetDetailedMeasurementAsync(string location, string parameter);
    

    }
}
