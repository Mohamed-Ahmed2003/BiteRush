namespace BiteRush.DTO;

public class MapPlacesResponse
{
    public int TotalCount { get; set; }
    public List<MapRestaurantDTO> Places { get; set; }
    public BoundingBoxDTO BoundingBox { get; set; }
}
