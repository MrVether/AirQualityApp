using AirQualityApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AirQualityApp.Services
{
    public class OpenAqApiClient
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private const string BaseUrl = "https://api.openaq.org/v2/";
        private readonly ILogger<OpenAqApiClient> _logger;
        private readonly AirQualityContext _context;

        public OpenAqApiClient(ILogger<OpenAqApiClient> logger, AirQualityContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<List<Country>> GetCountriesAsync()
        {
            var endpoint = "countries";
            var jsonResponse = await GetDataAsync(endpoint, "");
            var countriesResponse = JsonConvert.DeserializeObject<CountriesResponse>(jsonResponse);
            var countries = countriesResponse.Results;

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

        public List<Country> GetCountriesFromDb()
        {
            return _context.Countries.ToList();
        }

        public async Task<List<Measurement>> GetGlobalMeasurementsAsync(string parameter, string country)
        {
            var endpoint = "measurements";
            var measurements = new List<Measurement>();
            var parameters = $"country={country}&parameter={parameter}";

            var jsonResponse = await GetDataAsync(endpoint, parameters);
            var measurementsResponse = JsonConvert.DeserializeObject<MeasurementsResponse>(jsonResponse);
            if (measurementsResponse != null && measurementsResponse.Results != null)
            {
                measurements.AddRange(measurementsResponse.Results);
            }
            // Save to DB
            foreach (var measurement in measurements)
            {
                var existingMeasurement = _context.Measurements
                    .Include(m => m.Date)
                    .Include(m => m.Coordinates)
                    .FirstOrDefault(m => m.Location == measurement.Location && m.Parameter == measurement.Parameter);
                if (existingMeasurement == null)
                {
                    Console.WriteLine("Adding new location");
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


        public List<Measurement> GetGlobalMeasurementsFromDb(string parameter, string country)
        {
            return _context.Measurements
                .Where(m => m.Parameter == parameter && m.Country == country)
                .Include(d => d.Date)
                .Include(c=>c.Coordinates)
                .ToList();
        }

        private async Task<string> GetDataAsync(string endpoint, string parameters)
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
                     //   _logger.LogInformation($"Successfully fetched data from {url}");
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


        public Dictionary<string, Dictionary<string, Measurement>> GroupMeasurementsByLocation(List<Measurement> measurements)
        {
            var locations = new Dictionary<string, Dictionary<string, Measurement>>();

            foreach (var measurement in measurements)
            {
                if (measurement != null)
                {
                    var location = measurement.Location;

                    if (!locations.ContainsKey(location))
                    {
                        locations[location] = new Dictionary<string, Measurement>();
                    }

                    var type = measurement.Parameter;

                    if (measurement.Date != null && (!locations[location].ContainsKey(type) ||
                                                     DateTime.Parse(locations[location][type].Date.Utc) < DateTime.Parse(measurement.Date.Utc)))
                    {
                        locations[location][type] = measurement;
                    }
                }
            }

            return locations;
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
}
