namespace BiteRush.DTO.OSRMResponse;

public class OsrmRouteResponse
{
    public string Code { get; set; } = string.Empty;
    public List<OsrmRoute> Routes { get; set; } = new();
    public List<OsrmWaypoint> Waypoints { get; set; } = new();
}

public class OsrmRoute
{
    public double Distance { get; set; }
    public double Duration { get; set; }
    public string Geometry { get; set; } = string.Empty; // encoded polyline
    public List<OsrmLeg> Legs { get; set; } = new();
}

public class OsrmLeg
{
    public double Distance { get; set; }
    public double Duration { get; set; }
    public string Summary { get; set; } = string.Empty;
}

public class OsrmWaypoint
{
    public string Name { get; set; } = string.Empty;
    public double[] Location { get; set; } = Array.Empty<double>(); // [lon, lat]
}