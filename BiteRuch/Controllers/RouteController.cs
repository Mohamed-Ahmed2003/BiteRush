using BiteRush.ServicesContract;
using Microsoft.AspNetCore.Mvc;

namespace BiteRush.Controllers;

[ApiController]
public class RouteController : ControllerBase
{
    private readonly IOSMRService _osrm;
    private readonly ILogger<RouteController> _logger;

    public RouteController(IOSMRService osrm, ILogger<RouteController> logger)
    {
        _osrm = osrm;
        _logger = logger;
    }


    /// <summary>
    /// Returns the driving route between the user's current location
    /// and a selected restaurant/café via OSRM + OpenStreetMap data.
    ///
    /// Response includes:
    ///   • encodedPolyline  — Google-encoded polyline string for flutter_map
    ///   • routePoints      — decoded [{ lat, lon }] list
    ///   • distanceMeters   — total route length
    ///   • durationSeconds  — estimated travel time
    ///   • distanceText     — human-readable distance (e.g. "1.4 km")
    ///   • durationText     — human-readable ETA  (e.g. "5 min")
    ///
    /// GET /api/route?fromLat=30.0626&amp;fromLon=31.2197&amp;toLat=29.9602&amp;toLon=31.2569
    /// </summary>
    [HttpGet]
    [Route("api/Route")]
    public async Task<IActionResult> GetRoute(
        [FromQuery] double fromLat,
        [FromQuery] double fromLon,
        [FromQuery] double toLat,
        [FromQuery] double toLon,
        CancellationToken ct = default)
    {
        if (!ValidateCoordinates(fromLat, fromLon, out var err1))
            return BadRequest(new { error = $"Origin: {err1}" });

        if (!ValidateCoordinates(toLat, toLon, out var err2))
            return BadRequest(new { error = $"Destination: {err2}" });
        try
        {
            var route = await _osrm.GetRouteAsync(fromLat, fromLon, toLat, toLon, ct);
            return Ok(route);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "OSRM HTTP error");
            return StatusCode(502, new { error = "Routing service is temporarily unavailable." });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "OSRM found no route");
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected routing error");
            return StatusCode(500, new { error = "An unexpected error occurred." });
        }
    }

    /// <summary>
    /// Live direction update — called by Flutter periodically (e.g. every 10 s)
    /// as the user moves towards the destination.
    ///
    /// The backend:
    ///   1. Checks whether the user is still on the current route polyline.
    ///   2. If yes: trims already-passed points, recalculates remaining distance.
    ///   3. If the user deviated (&gt; 50 m off-route): requests a fresh route from OSRM.
    ///
    /// Pass the currentEncodedPolyline from the previous response so the server
    /// can avoid an unnecessary OSRM round-trip when the user stays on course.
    ///
    /// GET /api/route/update
    ///   ?userLat=30.055&amp;userLon=31.225
    ///   &amp;destLat=29.9602&amp;destLon=31.2569
    ///   &amp;currentPolyline=_p~iF~ps|U_ulLnnqC_mqNvxq`@   (optional)
    /// </summary>

    [HttpGet]
    [Route("api/route/update")]
    public async Task<IActionResult> UpdateRoute(
        [FromQuery] double userLat,
        [FromQuery] double userLon,
        [FromQuery] double destLat,
        [FromQuery] double destLon,
        [FromQuery] string? currentPolyline = null,
        CancellationToken ct = default)
    {
        if (!ValidateCoordinates(userLat, userLon, out var err1))
            return BadRequest(new { error = $"User location: {err1}" });

        if (!ValidateCoordinates(destLat, destLon, out var err2))
            return BadRequest(new { error = $"Destination: {err2}" });

        try
        {
            var update = await _osrm.UpdateRouteAsync(
                userLat, userLon,
                destLat, destLon,
                currentPolyline,
                ct);

            return Ok(update);
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "OSRM HTTP error during update");
            return StatusCode(502, new { error = "Routing service is temporarily unavailable." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during route update");
            return StatusCode(500, new { error = "An unexpected error occurred." });
        }
    }



    private static bool ValidateCoordinates(
       double lat, double lon, out string? error)
    {
        if (lat is < -90 or > 90)
        {
            error = "Latitude must be between -90 and 90.";
            return false;
        }
        if (lon is < -180 or > 180)
        {
            error = "Longitude must be between -180 and 180.";
            return false;
        }
        error = null;
        return true;
    }
}
