namespace BiteRush.DTO;

public class RouteResponseDTO
{
    /// <summary>Google-encoded polyline returned directly from OSRM.</summary>
    public string EncodedPolyline { get; set; } = string.Empty;

    /// <summary>Decoded list of coordinate points along the route.</summary>
    public List<CoordinateDTO> RoutePoints { get; set; } = new();

    public double DistanceMeters { get; set; }
    public double DurationSeconds { get; set; }

    // Convenience strings for display
    public string DistanceText => DistanceMeters >= 1000
        ? $"{DistanceMeters / 1000:F1} km"
        : $"{DistanceMeters:F0} m";

    public string DurationText
    {
        get
        {
            var mins = (int)Math.Round(DurationSeconds / 60);
            return mins < 60 ? $"{mins} min" : $"{mins / 60}h {mins % 60}min";
        }
    }
}
