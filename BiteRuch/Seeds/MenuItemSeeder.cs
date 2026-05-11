using BiteRush.Models;
using Microsoft.EntityFrameworkCore;

namespace BiteRush.Seeds;

public static class MenuItemSeeder
{
    public static readonly MenuItem[] menuItems =
    [
        new (SeedIds.KosharyId, "Koshary", 40.0, "/images/menuitems/koshary.jpg",true,SeedIds.KosharyElTahrirId),
        new (SeedIds.FalafelId, "Falafel", 30.0, "/images/menuitems/falafel.jpg",true,SeedIds.ZoobaId),
        new (SeedIds.GrilledChickenId, "Grilled Chicken", 120.0, "/images/menuitems/grilledchicken.jpg",true,SeedIds.NileGrillId),
        new (SeedIds.MolokhiaId, "Molokhia", 50.0, "/images/menuitems/molokhia.jpg",true,SeedIds.CairoBitesId),
        new (SeedIds.KebabId, "Kebab", 60.0, "/images/menuitems/kebab.jpg",true,SeedIds.CairoBitesId)
    ];
    public static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MenuItem>().HasData(menuItems);
    }
}
