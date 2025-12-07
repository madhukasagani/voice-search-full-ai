using Microsoft.EntityFrameworkCore;
using VoiceSearch.Api.Models;

namespace VoiceSearch.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> opts) : base(opts) { }
    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Sku = "SKU-1001", Name = "Running Shoes - SpeedX", Description = "Lightweight running shoes for daily training." },
            new Product { Id = 2, Sku = "SKU-1002", Name = "Casual Sneakers - RelaxPro", Description = "Stylish casual sneakers for everyday use." },
            new Product { Id = 3, Sku = "SKU-1003", Name = "Trail Running Shoes - MountainGrip", Description = "Rugged shoes for trail running with extra grip." }
        );
    }
}
