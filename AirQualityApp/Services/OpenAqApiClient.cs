using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace AirQualityApp.Services
{
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
        public Date Date { get; set; } // Add this line

        [JsonProperty("coordinates")]
        public Coordinates Coordinates { get; set; }
    }


    public class OpenAqApiClient
    {
        private static readonly HttpClient HttpClient = new HttpClient();
        private const string BaseUrl = "https://api.openaq.org/v2/";

        public async Task<string> GetDataAsync(string endpoint, string parameters)
        {
            var url = $"{BaseUrl}{endpoint}?{parameters}";
            var response = await HttpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new HttpRequestException($"Error fetching data from OpenAQ API. Status Code: {response.StatusCode}");
            }
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
