using AirQualityApp.Models;
using Microsoft.AspNetCore.SignalR;

namespace AirQualityApp.Hubs
{
    public class MapHub : Hub
    {
        public async Task SendMeasurementsAsync(Dictionary<string, Measurement> measurementsByLocation)
        {
            await Clients.All.SendAsync("ReceiveMeasurements", measurementsByLocation);
        }
    }
}
