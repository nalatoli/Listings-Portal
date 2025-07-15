using Listings_Portal.Lib.Models.Cloud;
using Listings_Portal.Lib.Tools.Cloud.RentCast;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Listings_Portal.Lib.Models.Cloud
{
    /// <summary>
    /// Rent/Sale listing cloud model.
    /// </summary>
    public class ListingCloud
    {
        /// <summary>
        /// Listing ID (i.e. "2005-Arborside-Dr,-Austin,-TX-78754")
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// Address line #1 (i.e. "3821 Hargis St")
        /// </summary>
        public required string AddressLine1 { get; set; }

        /// <summary>
        /// Address line #2 (if applicable).
        /// </summary>
        public string? AddressLine2 { get; set; }

        /// <summary>
        /// Listing's property city (i.e. "Austin").
        /// </summary>
        public required string City { get; set; }

        /// <summary>
        /// Listing's property state (i.e. "TX").
        /// </summary>
        public required string State { get; set; }

        /// <summary>
        /// Listing's zipcode (i.e. "78723").
        /// </summary>
        public required string ZipCode { get; set; }

        /// <summary>
        /// Listing's property county (i.e. "Travis").
        /// </summary>
        public required string County { get; set; }

        /// <summary>
        /// Listing's latitude (i.e. 30.290643).
        /// </summary>
        public required double Latitude { get; set; }

        /// <summary>
        /// Listing's longitude (i.e. -97.701547).
        /// </summary>
        public required double Longitude { get; set; }

        /// <summary>
        /// Listing's property type (i.e. "Single Family").
        /// </summary>
        public required string PropertyType { get; set; }

        /// <summary>
        /// Number of bedrooms in listing (i.e. 4).
        /// </summary>
        public double Bedrooms { get; set; } = -1;

        /// <summary>
        /// Number of bathrooms in listing (i.e. 4).
        /// </summary>
        public double Bathrooms { get; set; } = -1;

        /// <summary>
        /// Listing property's square footage (i.e. 2345).
        /// </summary>
        public int SquareFootage { get; set; } = -1;

        /// <summary>
        /// Listing lot size (i.e. 3284).
        /// </summary>
        public int LotSize { get; set; } = -1;

        /// <summary>
        /// Year property was built (i.e. 2008).
        /// </summary>
        public int YearBuilt { get; set; } = -1;

        /// <summary>
        /// HOA info for property.
        /// </summary>
        public HoaCloud? Hoa { get; set; }

        /// <summary>
        /// Listing's activity status (i.e. "Active").
        /// </summary>
        public required string Status { get; set; }

        /// <summary>
        /// Property price (i.e. 899000).
        /// </summary>
        public required long Price { get; set; }

        /// <summary>
        /// Listing type (i.e. "Standard").
        /// </summary>
        public required string ListingType { get; set; }

        /// <summary>
        /// Listed date (i.e. "2024-09-18T00:00:00.000Z").
        /// </summary>
        public required DateTime ListedDate { get; set; }

        /// <summary>
        /// Number of days listing has been on market (i.e. 90).
        /// </summary>
        public required long DaysOnMarket { get; set; }

        /// <summary> Listing's MLS name (i.e. "CentralTexas"). </summary>
        public string? MlsName { get; set; }

        /// <summary> Listing's MLS number (i.e. "556965"). </summary>
        public string? MlsNumber { get; set; }

        /// <summary> Listing's agent. </summary>
        public RealtorCloud? ListingAgent { get; set; }

        /// <summary> Listing's office. </summary>
        public RealtorCloud? ListingOffice { get; set; }
    }
}
