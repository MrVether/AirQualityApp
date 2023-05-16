using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Diagnostics.Metrics;
using System.Web;

namespace AirQualityApp.Services
{
    public class Country
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }
    public class Coordinates
    {
        [JsonProperty("latitude")]
        public double Latitude { get; set; }

        [JsonProperty("longitude")]
        public double Longitude { get; set; }
    }

    public class Date
    {
        [JsonProperty("utc")]
        public string Utc { get; set; }

        [JsonProperty("local")]
        public string Local { get; set; }
    }

    public class Measurement
    {
        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("City")]
        public string City { get; set; }

        [JsonProperty("parameter")]
        public string Parameter { get; set; }

        [JsonProperty("value")]
        public double Value { get; set; }

        [JsonProperty("unit")]
        public string Unit { get; set; }

        [JsonProperty("date")]
        public Date Date { get; set; }

        [JsonProperty("coordinates")]
        public Coordinates Coordinates { get; set; }
    }

    public class OpenAqApiClient
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private const string BaseUrl = "https://api.openaq.org/v2/";
        private readonly ILogger<OpenAqApiClient> _logger;

        public OpenAqApiClient(ILogger<OpenAqApiClient> logger)
        {
            _logger = logger;
        }
        public class CountriesResponse
        {
            [JsonProperty("results")]
            public List<Country> Results { get; set; }
        }

        public async Task<List<Country>> GetCountriesAsync()
        {
            var endpoint = "countries";
            var jsonResponse = await GetDataAsync(endpoint, "");
            var countriesResponse = JsonConvert.DeserializeObject<CountriesResponse>(jsonResponse);
            return countriesResponse.Results;
        }
        public async Task<string> GetDataAsync(string endpoint, string parameters)
        {
            var url = $"{BaseUrl}{endpoint}?{parameters}";
            var response = await HttpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Successfully fetched data from {url}. Response: {content}");
                _logger.LogInformation("");
                _logger.LogInformation("");
                _logger.LogInformation("");
                _logger.LogInformation("");
                _logger.LogInformation("");
                _logger.LogInformation("");
                return content;
            }
            else
            {
                var error = $"Error fetching data from OpenAQ API at {url}. Status Code: {response.StatusCode}";
                _logger.LogError(error);
                throw new HttpRequestException(error);
            }
        }

        public async Task<List<Measurement>> GetGlobalMeasurementsAsync(string parameter, string country)
        {
            var logger = LoggerFactory.Create(builder =>
            {
                builder
                    .AddConsole((options) => { })
                    .AddFilter((provider, category, logLevel) =>
                    {
                        if (provider == "Microsoft.Extensions.Logging.Console.ConsoleLoggerProvider" &&
                            category == "Microsoft.AspNetCore.Http.Request")
                        {
                            return false;
                        }
                        return true;
                    });
            });

            var endpoint = "measurements";
            var measurements = new List<Measurement>();
            int page = 1;
            int limit = 1000; // Set the limit to a safe value
            int maxRetries = 3; // Maximum number of retries
            string dateFrom = HttpUtility.UrlEncode(DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-ddTHH:mm:sszzz"));
            string dateTo = HttpUtility.UrlEncode(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:sszzz"));



            while (true)
            {
                var parameters = $"country={country}&parameter={parameter}&limit={limit}&page={page}&date_from={dateFrom}&date_to={dateTo}";

                string jsonResponse = null;
                for (int i = 0; i < maxRetries; i++)
                {
                    try
                    {
                        _logger.LogInformation($"Sending request: {endpoint} {parameters}");
                        jsonResponse = await GetDataAsync(endpoint, parameters);
                        break; // If the call was successful, break out of the retry loop
                    }
                    catch (HttpRequestException ex)
                    {
                        // If we've reached the maximum number of retries, rethrow the exception
                        if (i == maxRetries - 1)
                        {
                            _logger.LogError($"Error fetching data from OpenAQ API at {endpoint} {parameters}. Error: {ex.Message}");
                            return new List<Measurement>();
                        }

                        // Wait 1 second before the next retry
                        _logger.LogWarning($"Error fetching data from OpenAQ API at {endpoint} {parameters}. Retrying in 10 seconds...");
                        await Task.Delay(10000);
                    }
                }

                var measurementsResponse = JsonConvert.DeserializeObject<MeasurementsResponse>(jsonResponse);

                if (measurementsResponse.Results.Count == 0)
                {
                    break;
                }

                measurements.AddRange(measurementsResponse.Results);
                page++;
                await Task.Delay(1000); // Wait 1 second between requests to avoid hitting the rate limit
            }

            return measurements;
        }



        public Dictionary<string, Dictionary<string, Measurement>> GroupMeasurementsByLocation(List<Measurement> measurements)
        {
            var locations = new Dictionary<string, Dictionary<string, Measurement>>();

            foreach (var measurement in measurements)
            {
                var location = measurement.Location;

                if (!locations.ContainsKey(location))
                {
                    locations[location] = new Dictionary<string, Measurement>();
                }

                var type = measurement.Parameter;

                if (!locations[location].ContainsKey(type) ||
                    DateTime.Parse(locations[location][type].Date.Utc) < DateTime.Parse(measurement.Date.Utc))
                {
                    locations[location][type] = measurement;
                }
            }

            return locations;
        }

        public async Task<List<Measurement>> GetMeasurementsAsync(string city, string parameter, int limit)
        {
            var endpoint = "measurements";
            var parameters = $"city={city}&parameter={parameter}&limit={limit}";

            var jsonResponse = await GetDataAsync(endpoint, parameters);

            var measurementsResponse = JsonConvert.DeserializeObject<MeasurementsResponse>(jsonResponse);

            return measurementsResponse.Results;
        }

        public class MeasurementsResponse
        {


            [JsonProperty("results")]
            public List<Measurement> Results { get; set; }
        }


    }
}
