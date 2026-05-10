using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BiteRush.Models;

public class MenuItem
{
    [Key]
    public Guid Id { get; set; }
    [StringLength(50)]
    public string? Name { get; set; }
    [Required]
    public double Price { get; set; }

    [Url]
    public string? ImageUrl { get; set; }

    [Required]
    public bool? IsAvaiable { get; set; }

    [Required]
    [ForeignKey("Restaurant")]
    public Guid RestaurantId { get; set; }

    public readonly Restaurant restaurant;


}
