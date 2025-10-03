using Listings_Portal.Lib.Models.Cloud;
using Listings_Portal.Lib.Tools.Cloud.RentCast;
using NetTopologySuite.Geometries;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Listings_Portal.Lib.Models.Entities
{
    /// <summary>
    /// Rent/sale listing model.
    /// </summary>
    public class Listing
    {
        /// <summary>
        /// Listing ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Listing GUID (i.e. "2005-Arborside-Dr,-Austin,-TX-78754")
        /// </summary>
        public required string Guid { get; set; }

        /// <summary>
        /// Listing's type.
        /// </summary>
        public required ListingType Type { get; set; }

        /// <summary>
        /// Address line #1 (i.e. "3821 Hargis St")
        /// </summary>
        public required string AddressLine1 { get; set; }

        /// <summary>
        /// Address line #2 (if applicable).
        /// </summary>
        public required string? AddressLine2 { get; set; }

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
        /// Listing's geospatial location.
        /// </summary>
        public required Point Location { get; set; }

        /// <summary>
        /// Listing's property type (i.e. "Single Family").
        /// </summary>
        public required string PropertyType { get; set; }

        /// <summary>
        /// Number of bedrooms in listing (i.e. 4).
        /// </summary>
        public required double Bedrooms { get; set; }

        /// <summary>
        /// Number of bathrooms in listing (i.e. 4).
        /// </summary>
        public required double Bathrooms { get; set; }

        /// <summary>
        /// Listing property's square footage (i.e. 2345).
        /// </summary>
        public required int SquareFootage { get; set; }

        /// <summary>
        /// Listing lot size (i.e. 3284).
        /// </summary>
        public required int LotSize { get; set; }

        /// <summary>
        /// Year property was built (i.e. 2008).
        /// </summary>
        public required int YearBuilt { get; set; }

        /// <summary>
        /// HOA info for property.
        /// </summary>
        public required Hoa? Hoa { get; set; }

        /// <summary>
        /// Listing's activity status (i.e. "Active").
        /// </summary>
        public required string Status { get; set; }

        /// <summary>
        /// Property price (i.e. 899000).
        /// </summary>
        public required long Price { get; set; }

        /// <summary>
        /// Listing type (i.e. Standard).
        /// </summary>
        public required string ListingType { get; set; }

        /// <summary>
        /// Listed date (i.e. "2024-09-18T00:00:00.000Z").
        /// </summary>
        public required DateTime ListedDate { get; set; }

        /// <summary> 
        /// Listing's MLS name (i.e. "CentralTexas"). 
        /// </summary>
        public required string? MlsName { get; set; }

        /// <summary> 
        /// Listing's MLS number (i.e. "556965"). 
        /// </summary>
        public required string? MlsNumber { get; set; }

        /// <summary> 
        /// Listing's agent. 
        /// </summary>
        public required RealtorAgent? ListingAgent { get; set; }

        /// <summary> 
        /// Listing's office. 
        /// </summary>
        public required RealtorOffice? ListingOffice { get; set; }

        /// <summary>
        /// Gets listing model.
        /// </summary>
        /// <param name="cloudModel"> Cloud model. </param>
        /// <returns> Listing DB model. </returns>
        public static Listing FromCloud(ListingCloud cloudModel)
        {
            return new Listing()
            {
                Guid = cloudModel.Id,
                Type = cloudModel.Price < 10000 ? Tools.Cloud.RentCast.ListingType.Rent : Tools.Cloud.RentCast.ListingType.Sale,
                AddressLine1 = cloudModel.AddressLine1,
                AddressLine2 = cloudModel.AddressLine2,
                City = cloudModel.City,
                State = cloudModel.State,
                ZipCode = cloudModel.ZipCode,
                County = cloudModel.County,
                Location = GetPoint(cloudModel.Longitude, cloudModel.Latitude),
                PropertyType = cloudModel.PropertyType,
                Bedrooms = cloudModel.Bedrooms,
                Bathrooms = cloudModel.Bathrooms,
                SquareFootage = cloudModel.SquareFootage,
                LotSize = cloudModel.LotSize,
                YearBuilt = cloudModel.YearBuilt,
                Hoa = Hoa.FromCloud(cloudModel.Hoa),
                Status = cloudModel.Status,
                Price = cloudModel.Price,
                ListingType = cloudModel.ListingType,
                ListedDate = cloudModel.ListedDate,
                MlsName = cloudModel.MlsName,
                MlsNumber = cloudModel.MlsNumber,
                ListingAgent = RealtorAgent.FromCloud(cloudModel.ListingAgent),
                ListingOffice = RealtorOffice.FromCloud(cloudModel.ListingOffice),
            };
        }

        /// <summary>
        /// Gets geospatical point.
        /// </summary>
        /// <param name="longitude"> Longitude. </param>
        /// <param name="latitude"> Latitude. </param>
        /// <returns> Geospatial point. </returns>
        public static Point GetPoint(double longitude, double latitude)
            => new Point(longitude, latitude) { SRID = 4326 };
    }
}
