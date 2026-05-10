using BiteRush.DTO;

namespace BiteRush.ServicesContract;

public interface IOSMRService
{
    public Task<RouteResponseDTO> GetRouteAsync(
       double fromLat, double fromLon,
       double toLat, double toLon,
       CancellationToken ct = default);
    public Task<RouteUpdateResponseDTO> UpdateRouteAsync(
        double userLat, double userLon,
        double destLat, double destLon,
        string? currentEncodedPolyline,
        CancellationToken ct = default);

}
