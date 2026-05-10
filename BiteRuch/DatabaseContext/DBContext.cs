using BiteRush.Models;
using Microsoft.EntityFrameworkCore;

namespace BiteRush.DatabaseContext;

public class DBContext : DbContext
{

    public DBContext(DbContextOptions<DBContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Restaurant> Restaurants { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>();
        modelBuilder.Entity<Restaurant>()
            .HasMany(r => r.MenuItems)
            .WithOne()
            .HasForeignKey(m => m.RestaurantId);

        modelBuilder.Entity<MenuItem>();
    }

}
