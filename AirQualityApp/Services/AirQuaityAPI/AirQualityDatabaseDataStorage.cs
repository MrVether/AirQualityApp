using AirQualityApp.Interfaces.AirQualityAPI;
using AirQualityApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

public class AirQualityDatabaseDataStorage : IAirQualityDataStorage
{
    private readonly AirQualityContext _context;

    public AirQualityDatabaseDataStorage(AirQualityContext context)
    {
        _context = context;
    }

    public List<Country> GetCountriesFromDb()
    {
        return _context.Countries.ToList();
    }

    public List<Measurement> GetGlobalMeasurementsFromDb(string parameter, string country)
    {
        return _context.Measurements
            .Where(m => m.Parameter == parameter && m.Country == country)
            .Include(d => d.Date)
            .Include(c => c.Coordinates)
            .ToList();
    }

    public List<Marker> GetAQIMeasurmentFromDB(string parameter, string country)
    {
        return _context.Measurements
            .Where(m => m.Parameter == parameter && m.Country == country)
            .Select(m => new Marker
            {
                Location = m.Location,
                Parameter = m.Parameter,
                Value = m.Value,
                Latitude = m.Coordinates.Latitude,
                Longitude = m.Coordinates.Longitude
            })
            .ToList();
    }

    public async Task<List<Measurement>> FetchMeasurementsFromDb()
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        Console.WriteLine("Benchmarking FetchMeasurementsFromDb..");
        var measurements = new List<Measurement>();
        var countries = GetCountriesFromDb();

        foreach (var country in countries)
        {
            var pm10 = GetGlobalMeasurementsFromDb("pm10", country.Code);
            var pm25 = GetGlobalMeasurementsFromDb("pm25", country.Code);
            var no2 = GetGlobalMeasurementsFromDb("no2", country.Code);
            var o3 = GetGlobalMeasurementsFromDb("o3", country.Code);
            var co = GetGlobalMeasurementsFromDb("co", country.Code);
            var so2 = GetGlobalMeasurementsFromDb("so2", country.Code);
            var bc = GetGlobalMeasurementsFromDb("bc", country.Code);
            var nh3 = GetGlobalMeasurementsFromDb("nh3", country.Code);
            var pm1 = GetGlobalMeasurementsFromDb("pm1", country.Code);
            var pm4 = GetGlobalMeasurementsFromDb("pm4", country.Code);
            var no = GetGlobalMeasurementsFromDb("no", country.Code);
            var nox = GetGlobalMeasurementsFromDb("nox", country.Code);
            var ch4 = GetGlobalMeasurementsFromDb("ch4", country.Code);
            var co2 = GetGlobalMeasurementsFromDb("co2", country.Code);


            measurements.AddRange(pm10);
            measurements.AddRange(pm25);
            measurements.AddRange(no2);
            measurements.AddRange(o3);
            measurements.AddRange(co);
            measurements.AddRange(so2);
            measurements.AddRange(bc);
            measurements.AddRange(nh3);
            measurements.AddRange(pm1);
            measurements.AddRange(pm4);
            measurements.AddRange(no);
            measurements.AddRange(nox);
            measurements.AddRange(ch4);
            measurements.AddRange(co2);
        }
        stopwatch.Stop();
        Console.WriteLine($"Benchmarking FetchMeasurementsFromDb.. Done! Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
        return measurements;
    }

    public async Task<List<Marker>> FetchAQIMeasurementsFromDb()
    {
        Console.WriteLine("Benchmarking FetchAQIMeasurementsFromDb..");
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        var measurements = new List<Marker>();
        var countries = GetCountriesFromDb();

        foreach (var country in countries)
        {
            var pm10 = GetAQIMeasurmentFromDB("pm10", country.Code);
            var pm25 = GetAQIMeasurmentFromDB("pm25", country.Code);
            var no2 = GetAQIMeasurmentFromDB("no2", country.Code);
            var o3 = GetAQIMeasurmentFromDB("o3", country.Code);
            var co = GetAQIMeasurmentFromDB("co", country.Code);
            var so2 = GetAQIMeasurmentFromDB("so2", country.Code);



            measurements.AddRange(pm10);
            measurements.AddRange(pm25);
            measurements.AddRange(no2);
            measurements.AddRange(o3);
            measurements.AddRange(co);
            measurements.AddRange(so2);

        }
        stopwatch.Stop();
        Console.WriteLine($"Benchmarking FetchAQIMeasurementsFromDb.. Done! Elapsed time: {stopwatch.ElapsedMilliseconds} ms");
        return measurements;
    }

    public async Task<Measurement> GetDetailedMeasurementAsync(string location, string parameter)
    {
        var measurement = _context.Measurements
            .Include(m => m.Date)
            .Include(m => m.Coordinates)
            .FirstOrDefault(m => m.Location == location && m.Parameter == parameter);

        if (measurement == null)
        {

        }

        return measurement;
    }


}