using Bogus;
using Listings_Portal.Lib.Models.Entities;
using Listings_Portal.Lib.Tools.Cloud.RentCast;

namespace Listings_Portal.Tests.Tools
{
    internal partial class SampleData
    {
        public const double DefaultLatDeg = 40.878379;
        public const double DefaultLonDeg = -73.881924;
        private static readonly string[] counties = ["Kings", "New York", "Bronx"];

        /// <summary>
        /// Creates listing faker. 
        /// </summary>
        /// <param name="listingType"> Listing type. </param>
        /// <param name="latitude"> Latitude of listings origin. </param>
        /// <param name="latitude"> Longitude of listings origin. </param>
        /// <param name="maxRadius"> Maxium radius of listings from origin (in miles). </param>
        /// <returns> Listing faker. </returns>
        public static Faker<Listing> CreateListingFaker(
            ListingType listingType = ListingType.Rent, 
            double latitude = DefaultLatDeg,
            double longitude = DefaultLonDeg,
            double maxRadius = 1)
        {
            return new Faker<Listing>()
                .RuleFor(l => l.AddressLine1, f => f.Address.StreetAddress())
                .RuleFor(l => l.AddressLine2, f =>
                {
                    if (f.Random.Bool(0.3f))
                    {
                        return f.PickRandom(new[]
                        {
                            $"Apt {f.Random.Int(1, 20)}{f.Random.Char('A', 'F')}",
                            $"Suite {f.Random.Int(100, 999)}",
                            $"{f.Random.Int(1, 10)}th Floor"
                        });
                    }
                    return null;
                })
                .RuleFor(l => l.City, f => f.Address.City())
                .RuleFor(l => l.State, _ => "NY")
                .RuleFor(l => l.County, f => f.PickRandom(counties))
                .RuleFor(l => l.ZipCode, f => f.Address.ZipCode("#####"))
                .RuleFor(l => l.Guid, (_, l) => string.Join(",-", [l.AddressLine1, l.AddressLine2, l.City, l.State, l.ZipCode]))
                .RuleFor(l => l.Type, f => listingType)
                .RuleFor(l => l.PropertyType, _ => "Apartment")
                .RuleFor(l => l.ListingType, _ => "Standard")
                .RuleFor(l => l.Status, _ => "Active")
                .RuleFor(l => l.Bedrooms, f => f.Random.Int(1, 9))
                .RuleFor(l => l.Bathrooms, f => f.Random.Int(1, 8))
                .RuleFor(l => l.Price, (f, l) => l.Type == ListingType.Sale ? f.Random.Int(9000, 2000000) : f.Random.Int(900, 10000))
                .RuleFor(l => l.ListedDate, f => f.Date.Recent(14, DateTime.UtcNow))
                .RuleFor(l => l.DaysOnMarket, f => f.Random.Int(1, 1000))
                .RuleFor(l => l.Location, f => {
                    const double R = 3958.8; // Earth radius, mi
                    var r = Math.Sqrt(f.Random.Double()) * Math.Max(0, maxRadius - 1E-4); 
                    var ang = f.Random.Double(0, 2 * Math.PI); 
                    var dLat = r / R * 180.0 / Math.PI * Math.Cos(ang); 
                    var dLon = r / R * 180.0 / Math.PI * Math.Sin(ang) / Math.Cos(latitude * Math.PI / 180);
                    return Listing.GetPoint(longitude + dLon, latitude + dLat);
                });
        }

    }
}