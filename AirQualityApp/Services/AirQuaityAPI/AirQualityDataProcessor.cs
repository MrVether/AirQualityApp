using AirQualityApp.Interfaces.AirQualityAPI;
using AirQualityApp.Models;
using System.Diagnostics;

namespace AirQualityApp.Services.AirQuaityAPI
{
    public class AirQualityDataProcessor : IAirQualityDataProcessor
    {
        public Dictionary<string, Dictionary<string, Measurement>> GroupMeasurementsByLocation(List<Measurement> measurements)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine("Benchmarking GroupMeasurementsByLocation..");
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
            stopwatch.Stop();
            Console.WriteLine($"Benchmarking GroupMeasurementsByLocation.. Done! Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
            return locations;

        }

        public Dictionary<string, Dictionary<string, Marker>> GroupMarkersByLocation(List<Marker> markers)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine("Benchmarking GroupMarkersByLocation..");
            var locations = new Dictionary<string, Dictionary<string, Marker>>();

            foreach (var marker in markers)
            {
                if (marker != null)
                {
                    var location = marker.Location;

                    if (!locations.ContainsKey(location))
                    {
                        locations[location] = new Dictionary<string, Marker>();
                    }

                    var type = marker.Parameter;

                    if (marker.Value != null && (!locations[location].ContainsKey(type)))
                    {
                        locations[location][type] = marker;
                    }
                }
            }
            stopwatch.Stop();
            Console.WriteLine($"Benchmarking GroupMarkersByLocation.. Done! Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
            return locations;
        }

        public List<Marker> CreateMarkersFromMeasurements(List<Marker> measurements)
        {
            var markers = new List<Marker>();

            foreach (var measurement in measurements)
            {
                var marker = new Marker
                {
                    Location = measurement.Location,
                    Country = measurement.Country,
                    Parameter = measurement.Parameter,
                    Latitude = measurement.Latitude,
                    Longitude = measurement.Longitude,
                    Value = measurement.Value
                };

                markers.Add(marker);
            }

            return markers;
        }
    }
}
