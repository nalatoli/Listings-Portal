using Listings_Portal.Lib.Models.Api.Dtos;
using Listings_Portal.Lib.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Listings_Portal.Tools
{
#pragma warning disable CS1591
    public class ListingsDbContext : DbContext
    {
        public DbSet<Listing> Listings { get; set; }

        public ListingsDbContext() { }

        public ListingsDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Listing>()
                .Property(l => l.Location)
                .HasColumnType("geometry (Point, 4326)"); 
        }
    }
#pragma warning restore CS1591
}
