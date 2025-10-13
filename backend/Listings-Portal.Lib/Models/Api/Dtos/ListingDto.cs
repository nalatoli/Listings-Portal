using Listings_Portal.Lib.Models.Entities;
using System.Linq.Expressions;

namespace Listings_Portal.Lib.Models.Api.Dtos
{
    /// <summary>
    /// Sale/rental Listing DTO.
    /// </summary>
    /// <param name="Id"> Listing ID. </param>
    /// <param name="Guid"> Listing GUID (i.e. "2005-Arborside-Dr,-Austin,-TX-78754"). </param>
    /// <param name="AddressLine1"> Address line #1 (i.e. "3821 Hargis St"). </param>
    /// <param name="AddressLine2"> Address line #2 (if applicable). </param>
    /// <param name="City"> Listing's property city (i.e. "Austin"). </param>
    /// <param name="State"> Listing's property state (i.e. "TX"). </param>
    /// <param name="ZipCode"> Listing's zipcode (i.e. "78723"). </param>
    /// <param name="County"> Listing's property county (i.e. "Travis"). </param>
    /// <param name="Latitude"> Listing's latitude (i.e. 30.290643). </param>
    /// <param name="Longitude"> Listing's longitude (i.e. -97.701547). </param>
    /// <param name="PropertyType"> Listing's property type (i.e. "Single Family"). </param>
    /// <param name="Bedrooms"> Number of bedrooms in listing (i.e. 4). </param>
    /// <param name="Bathrooms"> Number of bathrooms in listing (i.e. 4). </param>
    /// <param name="SquareFootage"> Listing property's square footage (i.e. 2345). </param>
    /// <param name="LotSize"> Listing lot size (i.e. 3284). </param>
    /// <param name="YearBuilt"> Year property was built (i.e. 2008). </param>
    /// <param name="Hoa"> Listing's activity status (i.e. "Active"). </param>
    /// <param name="Price"> Property price (i.e. 899000). </param>
    /// <param name="ListingType"> Listing type (i.e. Standard). </param>
    /// <param name="ListedDate"> Listed date (i.e. "2024-09-18T00:00:00.000Z"). </param>
    /// <param name="MlsName"> Listing's MLS name (i.e. "CentralTexas").  </param>
    /// <param name="MlsNumber"> Listing's MLS number (i.e. "556965"). </param>
    /// <param name="ListingAgent"> Listing's agent. </param>
    /// <param name="ListingOffice"> Listing's office. </param>
    public sealed record ListingDto(
        int Id, 
        string Guid,
        string AddressLine1, 
        string? AddressLine2, 
        string City, 
        string State, 
        string ZipCode, 
        string County, 
        double Latitude, 
        double Longitude, 
        string PropertyType, 
        double Bedrooms, 
        double Bathrooms, 
        int SquareFootage, 
        int LotSize, 
        int YearBuilt, 
        HoaDto? Hoa, 
        long Price, 
        string ListingType, 
        DateTime ListedDate,
        string? MlsName, 
        string? MlsNumber, 
        RealtorDto? ListingAgent,
        RealtorDto? ListingOffice)
    {
        /// <summary>
        /// Gets listing DTO model.
        /// </summary>
        /// <param name="entityModel"> Entity model. </param>
        /// <returns> Listing DTO  model. </returns>
        public static ListingDto FromEntity(Listing entityModel)
        {
            return new ListingDto(
                Id: entityModel.Id,
                Guid: entityModel.Guid,
                AddressLine1: entityModel.AddressLine1,
                AddressLine2: entityModel.AddressLine2,
                City: entityModel.City,
                State: entityModel.State,
                ZipCode: entityModel.ZipCode,
                County: entityModel.County,
                Latitude: entityModel.Location.Y,
                Longitude: entityModel.Location.X,
                PropertyType: entityModel.PropertyType,
                Bedrooms: entityModel.Bedrooms,
                Bathrooms: entityModel.Bathrooms,
                SquareFootage: entityModel.SquareFootage,
                LotSize: entityModel.LotSize,
                YearBuilt: entityModel.YearBuilt,
                Hoa: HoaDto.FromEntity(entityModel.Hoa),
                Price: entityModel.Price,
                ListingType: entityModel.ListingType,
                ListedDate: entityModel.ListedDate,
                MlsName: entityModel.MlsName,
                MlsNumber: entityModel.MlsNumber,
                ListingAgent: RealtorDto.FromEntity(entityModel.ListingAgent),
                ListingOffice: RealtorDto.FromEntity(entityModel.ListingOffice));
        }
    }
}
