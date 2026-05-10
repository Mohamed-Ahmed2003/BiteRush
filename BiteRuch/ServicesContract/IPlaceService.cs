using BiteRush.DTO;

namespace BiteRush.ServicesContract;

public interface IPlaceService
{
    public Task<MapPlacesResponse> GetPlacesForMapAsync(
        string productName,
        double userLat,
        double userLon,
        double radiusKm = 10.0);
}
