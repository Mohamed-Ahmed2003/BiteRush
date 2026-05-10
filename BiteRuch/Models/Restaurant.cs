using System.ComponentModel.DataAnnotations;

namespace BiteRush.Models;

public class Restaurant
{
    [Key]
    public Guid Id { get; set; }

    [StringLength(100)]
    public string? Name { get; set; }

    [StringLength(303)]
    public string? Description { get; set; }
    public double Rating { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string? ImageUrl { get; set; }
    public ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();



    public Restaurant(Guid id, string name, string description, double rating, double latitude, double longitude, string imageUrl)
    {

        Id = id;
        Name = name;
        Description = description;
        Rating = rating;
        Latitude = latitude;
        Longitude = longitude;
        ImageUrl = imageUrl;

    }
}
