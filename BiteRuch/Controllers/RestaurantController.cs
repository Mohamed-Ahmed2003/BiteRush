using BiteRush.DatabaseContext;
using BiteRush.ServicesContract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BiteRush.Controllers;

[ApiController]
public class RestaurantController(
    DBContext context,
    IPlaceService _placeService) : ControllerBase
{
    [Authorize]
    [HttpGet]
    [Route("api/restaurant/get")]
    public async Task<IActionResult> GetAllRestaurant()
    {
        var restaurants = await context.Restaurants.ToListAsync();
        return Ok(restaurants);
    }


    [Authorize]
    [HttpGet]
    [Route("api/restaurant/get/{resturantId}")]

    public async Task<IActionResult> GetMenuItem(Guid resturantId)
    {
        var restaurant = await context.Restaurants.
            Include(r => r.MenuItems)
            .FirstOrDefaultAsync(r => r.Id == resturantId);
        if (restaurant is null)
            return NotFound($"the restuarant wit Id {resturantId} not found");
        return Ok(restaurant.MenuItems);
    }

    [Authorize]
    [HttpGet]
    [Route("api/product/Map")]
    public async Task<IActionResult> getAllRestaurantWithProductMap(
        [FromQuery] string productName,
        [FromQuery] double userLat,
        [FromQuery] double userLon,
        [FromQuery] double radiusKm = 10.0)
    {
        if (string.IsNullOrWhiteSpace(productName))
            return BadRequest(new { error = "Query parameter 'product' is required." });

        if (userLat is < -90 or > 90)
            return BadRequest(new { error = "'userLat' must be between -90 and 90." });

        if (userLon is < -180 or > 180)
            return BadRequest(new { error = "'userLon' must be between -180 and 180." });

        if (radiusKm is <= 0 or > 100)
            return BadRequest(new { error = "'radiusKm' must be between 0 and 100." });
        var result = await _placeService.GetPlacesForMapAsync(
                productName, userLat, userLon, radiusKm);

        return Ok(result);
    }

    [Authorize]
    [HttpGet]
    [Route("api/product")]

    public async Task<IActionResult> GetAllRestaurantWithProduct(string productName)
    {
        var restaurants = await context.Restaurants
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

        if (!restaurants.Any())
            return NotFound("No restaurants found with this product.");

        return Ok(restaurants);
    }
}
