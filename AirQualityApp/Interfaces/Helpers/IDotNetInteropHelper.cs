using AirQualityApp.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AirQualityApp.Interfaces.Helpers

{
    public interface IDotNetInteropHelper
    {
        Task<List<Measurement>> GetMeasurementsAsync(string location);
    }
}
