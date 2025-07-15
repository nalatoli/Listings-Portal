using Listings_Portal.Lib.Models.Api.Dtos;
using Listings_Portal.Lib.Models.Cloud;
using Listings_Portal.Lib.Models.Entities;
using Listings_Portal.Tests.Tools.Managers;

namespace Listings_Portal.Tests.Tools
{
    internal partial class SampleData
    {
        public static ListingCloud CreateListingCloud(Action<ListingCloud>? configure = null)
        {
            return new ListingCloud
            {
                Id = "TEST123",
                AddressLine1 = "123 Main St",
                City = "Austin",
                State = "TX",
                ZipCode = "78701",
                County = "Travis",
                Latitude = 30.0,
                Longitude = -97.0,
                PropertyType = "Condo",
                Bedrooms = 3,
                Bathrooms = 2,
                SquareFootage = 2500,
                LotSize = 5000,
                YearBuilt = 2000,
                Hoa = CreateHoaCloud(),
                Status = "Active",
                Price = 2000,
                ListingType = "Rent",
                ListedDate = DateTime.UtcNow,
                DaysOnMarket = 5,
                MlsName = "CentralTexas",
                MlsNumber = "556700",
                ListingAgent = CreateRealtorCloud(r => r.Name = "Cool Agent"),
                ListingOffice = CreateRealtorCloud(r => r.Name = "Cool Office"),
            }.GetConfigured(configure);
        }

        public static Listing CreateListing(Action<Listing>? configure = null)
        {
            return new Listing
            {
                Id = 1,
                Guid = "TEST123",
                Type = Lib.Tools.Cloud.RentCast.ListingType.Rent,
                AddressLine1 = "123 Main St",
                AddressLine2 = null,
                City = "Austin",
                State = "TX",
                ZipCode = "78701",
                County = "Travis",
                Location = Listing.GetPoint(-97.0, 30.0),
                PropertyType = "Condo",
                Bedrooms = 3,
                Bathrooms = 2,
                SquareFootage = 2500,
                LotSize =  5000,
                YearBuilt = 2000,
                Hoa = CreateHoa(),
                Status = "Active",
                Price = 2000,
                ListingType = "Rent",
                ListedDate = DateTime.UtcNow,
                DaysOnMarket = 5,
                MlsName = "CentralTexas",
                MlsNumber = "556700",
                ListingAgent = CreateRealtor(r =>
                {
                    r.Id = 1;
                    r.Name = "Cool Agent";
                }),
                ListingOffice = CreateRealtor(r =>
                {
                    r.Id = 2;
                    r.Name = "Cool Office";
                }),
            }.GetConfigured(configure);
        }
    }
}