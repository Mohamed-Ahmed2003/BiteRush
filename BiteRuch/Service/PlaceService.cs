namespace BiteRush.Service;

using BiteRush.DatabaseContext;
using BiteRush.DTO;
using BiteRush.Helper;
using BiteRush.ServicesContract;
using Microsoft.EntityFrameworkCore;

public class PlacesService : IPlaceService
{
    private readonly DBContext _db;

    public PlacesService(DBContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Returns all restaurants/cafés that offer a given product,
    /// within <paramref name="radiusKm"/> of the user's location,
    /// sorted by straight-line distance ascending.
    ///
    /// The response includes a bounding box so Flutter's MapController
    /// can call fitBounds and frame all markers automatically.
    /// </summary>
    public async Task<MapPlacesResponse> GetPlacesForMapAsync(
        string productName,
        double userLat,
        double userLon,
        double radiusKm = 10.0)
    {

        // Pull candidate places that have the product available.
        // We do the radius filter in memory (Haversine) because SQLite doesn't
        // have spatial types. For SQL Server / PostGIS, swap this for a
        // NetTopologySuite spatial query for better performance.
        var restaurants = await _db.Restaurants
            .Where(r => r.MenuItems.Any(m =>
                m.Name.ToLower().Contains(productName.ToLower())))
            .Include(r => r.MenuItems
                .Where(m => m.Name.ToLower().Contains(productName.ToLower())))
            .Select(r => new
            {
                r.Id,
                r.Name,
                r.Description,
                r.Latitude,
                r.Longitude,
                r.ImageUrl,

                MatchingMenuItems = r.MenuItems.Select(m => new
                {
                    m.Id,
                    m.Name,
                    m.Price,
                    m.ImageUrl
                })
            })
            .ToListAsync();

        // Apply Haversine radius filter and project to DTOs
        var results = restaurants
            .Select(p => new
            {
                Place = p,
                DistanceKm = GeoHelper.HaversineDistanceKm(
                    userLat, userLon, p.Latitude, p.Longitude)
            })
            .Where(x => x.DistanceKm <= radiusKm)
            .OrderBy(x => x.DistanceKm)
            .Select(x => new MapRestaurantDTO
            {
                Latitude = x.Place.Latitude,
                Longitude = x.Place.Longitude,
                Name = x.Place.Name,
            })
            .ToList();

        // Build bounding box for flutter_map fitBounds
        var bbox = GeoHelper.ComputeBoundingBox(
            results.Select(r => (r.Latitude, r.Longitude)));

        return new MapPlacesResponse
        {
            TotalCount = results.Count,
            Places = results,
            BoundingBox = bbox
        };
    }

}
