using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BiteRuch.Migrations
{
    /// <inheritdoc />
    public partial class seeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "Id", "ImageUrl", "IsAvaiable", "Name", "Price", "RestaurantId" },
                values: new object[,]
                {
                    { new Guid("22222222-2222-2222-2222-222222222225"), "/images/menuitems/molokhia.jpg", true, "Molokhia", 50.0, new Guid("11111111-1111-1111-1111-111111111111") },
                    { new Guid("22222222-2222-2222-2222-222222222226"), "/images/menuitems/kebab.jpg", true, "Kebab", 60.0, new Guid("11111111-1111-1111-1111-111111111111") }
                });

            migrationBuilder.InsertData(
                table: "Restaurants",
                columns: new[] { "Id", "Description", "ImageUrl", "Latitude", "Longitude", "Name", "Rating" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111112"), "Famous Egyptian street food spot serving authentic koshary.", "/images/restaurants/koshary.jpg", 30.0444, 31.235700000000001, "Koshary El Tahrir", 4.5 },
                    { new Guid("11111111-1111-1111-1111-111111111113"), "Trendy Egyptian restaurant offering modern takes on local dishes.", "/images/restaurants/zooba.jpg", 30.045000000000002, 31.236000000000001, "Zooba", 4.2000000000000002 },
                    { new Guid("11111111-1111-1111-1111-111111111114"), "Classic Nile-side grill", "/images/restaurants/nilegrill.jpg", 30.050000000000001, 31.2333, "Nile Grill", 4.2000000000000002 }
                });

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "Id", "ImageUrl", "IsAvaiable", "Name", "Price", "RestaurantId" },
                values: new object[,]
                {
                    { new Guid("22222222-2222-2222-2222-222222222222"), "/images/menuitems/koshary.jpg", true, "Koshary", 40.0, new Guid("11111111-1111-1111-1111-111111111112") },
                    { new Guid("22222222-2222-2222-2222-222222222223"), "/images/menuitems/grilledchicken.jpg", true, "Grilled Chicken", 120.0, new Guid("11111111-1111-1111-1111-111111111114") },
                    { new Guid("22222222-2222-2222-2222-222222222224"), "/images/menuitems/falafel.jpg", true, "Falafel", 30.0, new Guid("11111111-1111-1111-1111-111111111113") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222223"));

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222224"));

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222225"));

            migrationBuilder.DeleteData(
                table: "MenuItems",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222226"));

            migrationBuilder.DeleteData(
                table: "Restaurants",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111112"));

            migrationBuilder.DeleteData(
                table: "Restaurants",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111113"));

            migrationBuilder.DeleteData(
                table: "Restaurants",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111114"));

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "Id", "ImageUrl", "IsAvaiable", "Name", "Price", "RestaurantId" },
                values: new object[] { new Guid("33333333-3333-3333-3333-333333333333"), "/images/menuitems/koshary.jpg", true, "Koshary", 40.0, new Guid("11111111-1111-1111-1111-111111111111") });

            migrationBuilder.InsertData(
                table: "Restaurants",
                columns: new[] { "Id", "Description", "ImageUrl", "Latitude", "Longitude", "Name", "Rating" },
                values: new object[] { new Guid("22222222-2222-2222-2222-222222222222"), "Classic Nile-side grill", "/images/restaurants/nilegrill.jpg", 30.050000000000001, 31.2333, "Nile Grill", 4.2000000000000002 });

            migrationBuilder.InsertData(
                table: "MenuItems",
                columns: new[] { "Id", "ImageUrl", "IsAvaiable", "Name", "Price", "RestaurantId" },
                values: new object[] { new Guid("44444444-4444-4444-4444-444444444444"), "/images/menuitems/grilledchicken.jpg", true, "Grilled Chicken", 120.0, new Guid("22222222-2222-2222-2222-222222222222") });
        }
    }
}
