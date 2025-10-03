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

            modelBuilder.Entity<Listing>()
                .HasIndex(l => l.Guid)
                .IsUnique();

            modelBuilder.Entity<Hoa>()
                .HasKey(r => r.ListingId);

            modelBuilder.Entity<RealtorAgent>()
                .HasKey(r => r.ListingId);

            modelBuilder.Entity<RealtorOffice>()
                .HasKey(r => r.ListingId);

            modelBuilder.Entity<Listing>()
                .HasOne(l => l.Hoa)
                .WithOne(h => h.Listing)
                .HasForeignKey<Hoa>(h => h.ListingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Listing>()
                .HasOne(l => l.ListingAgent)
                .WithOne(r => r.Listing)
                .HasForeignKey<RealtorAgent>(r => r.ListingId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Listing>()
                .HasOne(l => l.ListingOffice)
                .WithOne(r => r.Listing)
                .HasForeignKey<RealtorOffice>(r => r.ListingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
#pragma warning restore CS1591
}
