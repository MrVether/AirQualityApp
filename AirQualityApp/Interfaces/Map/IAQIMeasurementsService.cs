using AirQualityApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirQualityApp.Interfaces.Map
{
    public interface IAQIMeasurementsService
    {
        Task<List<Marker>> GetAQIMeasurementsAsync();
        Task<List<Measurement>> GetDetailedMeasurementsAsync();

    }
}
