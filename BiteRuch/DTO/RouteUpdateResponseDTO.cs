namespace BiteRush.DTO;

public class RouteUpdateResponseDTO
{
    public RouteResponseDTO Route { get; set; } = new();

    /// <summary>True when the user has deviated far enough to warrant a re-route.</summary>
    public bool IsRerouted { get; set; }

    /// <summary>Straight-line distance from user's current position to the destination (m).</summary>
    public double RemainingDistanceMeters { get; set; }

    public string RemainingDistanceText => RemainingDistanceMeters >= 1000
        ? $"{RemainingDistanceMeters / 1000:F1} km"
        : $"{RemainingDistanceMeters:F0} m";
}
