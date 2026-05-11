using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BiteRuch.Migrations
{
    /// <inheritdoc />
    public partial class SeedRestaurantsAndMenuItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Restaurants",
                columns: new[] { "Id", "Description", "ImageUrl", "Latitude", "Longitude", "Name", "Rating" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Trendy Cairo eatery", "/images/restaurants/cairobites.jpg", 30.0444, 31.235700000000001, "Cairo Bites", 4.5 },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Classic Nile-side grill", "/images/restaurants/nilegrill.jpg", 30.050000000000001, 31.2333, "Nile Grill", 4.2000000000000002 }
                });

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "Id", "ImageUrl", "IsAvaiable", "Name", "Price", "RestaurantId" },
                values: new object[,]
                {
                    { new Guid("33333333-3333-3333-3333-333333333333"), "/images/menuitems/koshary.jpg", true, "Koshary", 40.0, new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("44444444-4444-4444-4444-444444444444"), "/images/menuitems/grilledchicken.jpg", true, "Grilled Chicken", 120.0, new Guid("22222222-2222-2222-2222-222222222222") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"));

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"));

            migrationBuilder.DeleteData(
                table: "Restaurants",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Restaurants",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));
        }
    }
}
