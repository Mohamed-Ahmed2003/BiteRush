using BiteRush.DTO;
using BiteRush.DTO.OSRMResponse;
using BiteRush.Helper;
using BiteRush.ServicesContract;
using System.Text.Json;

namespace BiteRush.Service;

public class OSRMService : IOSMRService
{

    /// <summary>
    /// Typed HttpClient service that communicates with an OSRM routing engine.
    ///
    /// Default: uses the public OSRM demo server (good for dev/testing).
    /// Production: point OsrmBaseUrl in appsettings.json at your self-hosted
    ///   Docker instance: docker run -t -v "${PWD}:/data" osrm/osrm-backend …
    /// </summary>

    private readonly HttpClient _http;
    private readonly ILogger<OSRMService> _logger;

    // Deviation threshold: if the user wanders more than this (metres) off the
    // current route, we request a fresh route from OSRM.
    private const double RerouteThresholdMeters = 50.0;

    public OSRMService(HttpClient http, ILogger<OSRMService> logger)
    {
        _http = http;
        _logger = logger;
    }

    /// <summary>
    /// Requests a driving route between two coordinate pairs from OSRM.
    /// Returns the encoded polyline, decoded points, distance and duration.
    /// </summary>
    public async Task<RouteResponseDTO> GetRouteAsync(
        double fromLat, double fromLon,
        double toLat, double toLon,
        CancellationToken ct = default)
    {
        // OSRM coordinate order is lon,lat  (note: reversed from lat,lon!)
        var url = $"route/v1/driving/{fromLon},{fromLat};{toLon},{toLat}"
                + "?overview=full&geometries=polyline&steps=false";

        _logger.LogInformation("OSRM request: {Url}", url);

        var response = await _http.GetAsync(url, ct);
        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync(ct);
        var osrm = JsonSerializer.Deserialize<OsrmRouteResponse>(json,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (osrm is null || osrm.Code != "Ok" || osrm.Routes.Count == 0)
        {
            _logger.LogWarning("OSRM returned no routes. Code: {Code}", osrm?.Code);
            throw new InvalidOperationException(
                $"OSRM could not find a route. Code: {osrm?.Code ?? "null"}");
        }

        var best = osrm.Routes[0];
        var points = GeoHelper.DecodePolyline(best.Geometry);

        return new RouteResponseDTO
        {
            EncodedPolyline = best.Geometry,
            RoutePoints = points,
            DistanceMeters = best.Distance,
            DurationSeconds = best.Duration
        };
    }

    /// <summary>
    /// Re-evaluates the route from the user's current position.
    /// Returns a <see cref="RouteUpdateResponseDTO"/> that tells Flutter:
    ///   - Whether a full re-route was needed (user deviated)
    ///   - The updated/current route
    ///   - Remaining straight-line distance to destination
    /// </summary>
    public async Task<RouteUpdateResponseDTO> UpdateRouteAsync(
        double userLat, double userLon,
        double destLat, double destLon,
        string? currentEncodedPolyline,
        CancellationToken ct = default)
    {
        var remainingKm = GeoHelper.HaversineDistanceKm(
            userLat, userLon, destLat, destLon);

        bool isRerouted = false;
        RouteResponseDTO route;

        // Check whether the user has drifted off the existing polyline
        if (!string.IsNullOrWhiteSpace(currentEncodedPolyline)
            && !HasDeviatedFromRoute(userLat, userLon,
                                    currentEncodedPolyline,
                                    RerouteThresholdMeters))
        {
            // Still on route — return the same polyline without a new OSRM call
            var points = GeoHelper.DecodePolyline(currentEncodedPolyline);

            // Trim points already passed (behind the user) for a cleaner display
            var trimmed = TrimPassedPoints(points, userLat, userLon);

            // Recompute remaining distance along the trimmed polyline
            var remainingAlongRoute = PolylineDistanceMeters(trimmed);

            // Estimate remaining duration proportionally
            route = new RouteResponseDTO
            {
                EncodedPolyline = currentEncodedPolyline,
                RoutePoints = trimmed,
                DistanceMeters = remainingAlongRoute,
                DurationSeconds = remainingAlongRoute / 10.0 // ~36 km/h average
            };
        }
        else
        {
            // User deviated — request a fresh route from current position
            isRerouted = true;
            route = await GetRouteAsync(userLat, userLon, destLat, destLon, ct);
        }

        return new RouteUpdateResponseDTO
        {
            Route = route,
            IsRerouted = isRerouted,
            RemainingDistanceMeters = remainingKm * 1000
        };
    }

    // ── Private helpers ──────────────────────────────────────────────────────

    /// <summary>
    /// Returns true when the user's position is further than <paramref name="thresholdMeters"/>
    /// from every segment of the current route polyline.
    /// </summary>
    private static bool HasDeviatedFromRoute(
        double userLat, double userLon,
        string encodedPolyline,
        double thresholdMeters)
    {
        var points = GeoHelper.DecodePolyline(encodedPolyline);
        if (points.Count == 0) return true;

        double minDistKm = double.MaxValue;
        for (int i = 0; i < points.Count - 1; i++)
        {
            var d = PointToSegmentDistanceKm(
                userLat, userLon,
                points[i].Lat, points[i].Lon,
                points[i + 1].Lat, points[i + 1].Lon);

            if (d < minDistKm) minDistKm = d;
        }

        return minDistKm * 1000 > thresholdMeters;
    }

    /// <summary>
    /// Removes route points that are behind the user (already passed).
    /// Finds the point on the route closest to the user and returns
    /// everything from that index onward.
    /// </summary>
    private static List<CoordinateDTO> TrimPassedPoints(
        List<CoordinateDTO> points,
        double userLat, double userLon)
    {
        if (points.Count <= 1) return points;

        int closestIdx = 0;
        double minDist = double.MaxValue;

        for (int i = 0; i < points.Count; i++)
        {
            var d = GeoHelper.HaversineDistanceKm(
                userLat, userLon, points[i].Lat, points[i].Lon);
            if (d < minDist) { minDist = d; closestIdx = i; }
        }

        return points.Skip(closestIdx).ToList();
    }

    /// <summary>
    /// Calculates total path length (in metres) for a list of coordinate points.
    /// </summary>
    private static double PolylineDistanceMeters(List<CoordinateDTO> points)
    {
        double total = 0;
        for (int i = 0; i < points.Count - 1; i++)
        {
            total += GeoHelper.HaversineDistanceKm(
                points[i].Lat, points[i].Lon,
                points[i + 1].Lat, points[i + 1].Lon) * 1000;
        }
        return total;
    }

    /// <summary>
    /// Minimum distance (km) from point P to segment AB using vector projection.
    /// </summary>
    private static double PointToSegmentDistanceKm(
        double pLat, double pLon,
        double aLat, double aLon,
        double bLat, double bLon)
    {
        double dx = bLon - aLon;
        double dy = bLat - aLat;
        double lenSq = dx * dx + dy * dy;

        double t = 0;
        if (lenSq > 0)
            t = Math.Clamp(((pLon - aLon) * dx + (pLat - aLat) * dy) / lenSq, 0, 1);

        double closestLat = aLat + t * dy;
        double closestLon = aLon + t * dx;

        return GeoHelper.HaversineDistanceKm(pLat, pLon, closestLat, closestLon);
    }
}
