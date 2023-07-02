using Microsoft.JSInterop;

namespace AirQualityApp.Interfaces.Map
{
     public interface IMapInitializationService
        {
        Task<IJSObjectReference> InitializeMapAsync();
            }
    

}
