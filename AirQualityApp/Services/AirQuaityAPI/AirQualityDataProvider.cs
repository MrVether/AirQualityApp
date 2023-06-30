using AirQualityApp.Interfaces.AirQualityAPI;
using AirQualityApp.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

public class AirQualityDataProvider : IAirQualityDataProvider
{
    private static readonly HttpClient HttpClient = new HttpClient();
    private const string BaseUrl = "https://api.openaq.org/v2/";
    private readonly ILogger<AirQualityDataProvider> _logger;


    private readonly AirQualityContext _context;

    public AirQualityDataProvider(AirQualityContext context, ILogger<AirQualityDataProvider> logger)
    {
        _context = context;
        _logger = logger;
    }


    public async Task<List<Country>> GetCountriesAsync()
    {
        var endpoint = "countries";
        var countries = new List<Country>();
        var page = 1;
        const int limit = 100;
        CountriesResponse countriesResponse;

        do
        {
            var parameters = $"page={page}&limit={limit}";
            var jsonResponse = await GetDataAsync(endpoint, parameters);
            countriesResponse = JsonConvert.DeserializeObject<CountriesResponse>(jsonResponse);

            if (countriesResponse != null && countriesResponse.Results != null)
            {
                countries.AddRange(countriesResponse.Results);
            }
            page++;
        } while (countriesResponse != null && countriesResponse.Results.Count == limit);

        // Save to DB
        foreach (var country in countries)
        {
            if (_context.Countries.Find(country.Code) == null)
            {
                _context.Countries.Add(country);
            }
        }
        await _context.SaveChangesAsync();

        return countries;
    }
    public async Task<string> GetDataAsync(string endpoint, string parameters)
    {
        var url = $"{BaseUrl}{endpoint}?{parameters}";
        var attempts = 3;
        for (int i = 0; i < attempts; i++)
        {
            try
            {
                var response = await HttpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return content;
                }
                else
                {
                    var error = $"Error fetching data from OpenAQ API at {url}. Status Code: {response.StatusCode}. Attempt {i + 1} of {attempts}.";
                    _logger.LogError(error);
                    if (i == attempts - 1)
                    {
                        return string.Empty;
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Exception during fetching data from OpenAQ API at {url}. Attempt {i + 1} of {attempts}. Exception: {ex.Message}");
                if (ex is System.Net.Sockets.SocketException socketException && socketException.SocketErrorCode == System.Net.Sockets.SocketError.HostNotFound)
                {
                    _logger.LogError($"Host not found. Stopping further attempts.");
                    break;
                }
                else
                {
                    await Task.Delay(5000);
                    if (i == attempts - 1)
                    {
                        return string.Empty;
                    }
                }
            }
        }
        return string.Empty;
    }
    public async Task<List<Measurement>> GetGlobalMeasurementsAsync(string parameter, string country, TimeSpan timeSpan)
    {
        var endpoint = "measurements";
        var measurements = new List<Measurement>();
        var page = 1;
        const int limit = 1000;
        MeasurementsResponse measurementsResponse;
        var fromDate = DateTime.UtcNow.Add(-timeSpan).ToString("yyyy-MM-ddTHH:mm:ssZ");

        do
        {
            var parameters = $"country={country}&parameter={parameter}&date_from={fromDate}&page={page}&limit={limit}";
            var jsonResponse = await GetDataAsync(endpoint, parameters);
            measurementsResponse = JsonConvert.DeserializeObject<MeasurementsResponse>(jsonResponse);
            if (measurementsResponse != null && measurementsResponse.Results != null)
            {
                measurements.AddRange(measurementsResponse.Results.Select(m =>
                {
                    m.Country = country;
                    return m;
                }));
            }
            page++;
        } while (measurementsResponse != null && measurementsResponse.Results.Count == limit);



        // Save to DB
        foreach (var measurement in measurements)
        {
            var existingMeasurement = _context.Measurements
                .Include(m => m.Date)
                .Include(m => m.Coordinates)
                .FirstOrDefault(m => m.Location == measurement.Location && m.Parameter == measurement.Parameter);
            if (existingMeasurement == null)
            {
                var existingDate = _context.Dates.FirstOrDefault(d => d.Utc == measurement.Date.Utc && d.Local == measurement.Date.Local);
                var existingCoordinates = _context.Coordinates.FirstOrDefault(c => c.Latitude == measurement.Coordinates.Latitude && c.Longitude == measurement.Coordinates.Longitude);

                if (existingDate == null)
                {
                    _context.Dates.Add(measurement.Date);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    measurement.Date = existingDate;
                }

                if (existingCoordinates == null)
                {
                    _context.Coordinates.Add(measurement.Coordinates);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    measurement.Coordinates = existingCoordinates;
                }

                _context.Measurements.Add(measurement);
            }
            else
            {
                existingMeasurement.Value = measurement.Value;
                existingMeasurement.Date = _context.Dates.FirstOrDefault(d => d.Utc == measurement.Date.Utc && d.Local == measurement.Date.Local);
                existingMeasurement.Coordinates = _context.Coordinates.FirstOrDefault(c => c.Latitude == measurement.Coordinates.Latitude && c.Longitude == measurement.Coordinates.Longitude);
            }
        }
        await _context.SaveChangesAsync();

        return measurements;
    }
    public class CountriesResponse
    {
        [JsonProperty("results")]
        public List<Country> Results { get; set; }
    }

    public class MeasurementsResponse
    {
        [JsonProperty("results")]
        public List<Measurement> Results { get; set; }
    }
}