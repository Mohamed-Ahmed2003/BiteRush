using BiteRush.Models;
using Microsoft.EntityFrameworkCore;

namespace BiteRush.Seeds;

public static class RestaurantSeeder
{

    public static readonly Restaurant[] restaurants = [
        new (SeedIds.CairoBitesId,"Cairo Bites", "Trendy Cairo eatery", 4.5, 30.0444, 31.2357, "/images/restaurants/cairobites.jpg"),
        new (SeedIds.NileGrillId, "Nile Grill", "Classic Nile-side grill", 4.2, 30.0500, 31.2333, "/images/restaurants/nilegrill.jpg"),
        new (SeedIds.KosharyElTahrirId, "Koshary El Tahrir", "Famous Egyptian street food spot serving authentic koshary.",4.5,30.0444,31.2357,"/images/restaurants/koshary.jpg"),
        new (SeedIds.ZoobaId,"Zooba","Trendy Egyptian restaurant offering modern takes on local dishes.",4.2,30.0450,31.2360,"/images/restaurants/zooba.jpg")
    ];
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Restaurant>().HasData(restaurants);
    }
}
