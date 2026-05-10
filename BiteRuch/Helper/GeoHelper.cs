namespace BiteRush.Helper;

using BiteRush.DTO;


/// <summary>
/// Pure-static geographic utilities:
///   - Haversine straight-line distance
///   - Google-encoded polyline decoder
///   - Bounding-box builder
/// </summary>
public static class GeoHelper
{
    private const double EarthRadiusKm = 6371.0;

    /// <summary>
    /// Haversine formula — returns the great-circle distance in kilometres
    /// between two WGS-84 coordinate pairs.
    /// </summary>
    public static double HaversineDistanceKm(
        double lat1, double lon1,
        double lat2, double lon2)
    {
        var dLat = ToRadians(lat2 - lat1);
        var dLon = ToRadians(lon2 - lon1);

        var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
              + Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2))
              * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

        var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        return EarthRadiusKm * c;
    }

    /// <summary>
    /// Decodes a Google-encoded polyline string into a list of lat/lon pairs.
    /// OSRM returns this format when geometries=polyline is requested.
    /// Spec: https://developers.google.com/maps/documentation/utilities/polylinealgorithm
    /// </summary>
    public static List<CoordinateDTO> DecodePolyline(string encodedPolyline)
    {
        var points = new List<CoordinateDTO>();
        if (string.IsNullOrWhiteSpace(encodedPolyline)) return points;

        int index = 0, lat = 0, lng = 0;

        while (index < encodedPolyline.Length)
        {
            // Decode latitude
            int result = 0, shift = 0, b;
            do
            {
                b = encodedPolyline[index++] - 63;
                result |= (b & 0x1F) << shift;
                shift += 5;
            } while (b >= 0x20);

            lat += (result & 1) != 0 ? ~(result >> 1) : result >> 1;

            // Decode longitude
            result = 0; shift = 0;
            do
            {
                b = encodedPolyline[index++] - 63;
                result |= (b & 0x1F) << shift;
                shift += 5;
            } while (b >= 0x20);

            lng += (result & 1) != 0 ? ~(result >> 1) : result >> 1;

            points.Add(new CoordinateDTO
            {
                Lat = lat / 1e5,
                Lon = lng / 1e5
            });
        }

        return points;
    }

    /// <summary>
    /// Computes a tight bounding box around a collection of places
    /// so the Flutter map can call fitBounds.
    /// Adds a small padding fraction (default 10 %) around the edges.
    /// </summary>
    public static BoundingBoxDTO ComputeBoundingBox(
        IEnumerable<(double Lat, double Lon)> points,
        double paddingFraction = 0.10)
    {
        var list = points.ToList();
        if (list.Count == 0)
            return new BoundingBoxDTO();

        var minLat = list.Min(p => p.Lat);
        var maxLat = list.Max(p => p.Lat);
        var minLon = list.Min(p => p.Lon);
        var maxLon = list.Max(p => p.Lon);

        var latPad = (maxLat - minLat) * paddingFraction;
        var lonPad = (maxLon - minLon) * paddingFraction;

        // Ensure at least ~500 m padding when all results are the same point
        latPad = Math.Max(latPad, 0.005);
        lonPad = Math.Max(lonPad, 0.005);

        return new BoundingBoxDTO
        {
            SouthLat = minLat - latPad,
            WestLon = minLon - lonPad,
            NorthLat = maxLat + latPad,
            EastLon = maxLon + lonPad
        };
    }

    private static double ToRadians(double deg) => deg * Math.PI / 180.0;
}
