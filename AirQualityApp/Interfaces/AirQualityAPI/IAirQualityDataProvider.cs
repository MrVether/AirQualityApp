using AirQualityApp.Models;

namespace AirQualityApp.Interfaces.AirQualityAPI
{
    public interface IAirQualityDataProvider
    {
        Task<List<Country>> GetCountriesAsync();
        Task<List<Measurement>> GetGlobalMeasurementsAsync(string parameter, string country, TimeSpan timeSpan);


        Task<string> GetDataAsync(string endpoint, string parameters);
    }
}
