using Listings_Portal.Lib.Tools.Cloud.RentCast;
using System.Text.Json.Serialization;

namespace Listings_Portal.Lib.Models.Api
{
    public class SearchOptions
    {
        /// <summary>
        /// Search center's latitude (i.e. 30.290643).
        /// </summary>
        public required double Latitude { get; set; }

        /// <summary>
        /// Search center's longitude (i.e. -97.701547).
        /// </summary>
        public required double Longitude { get; set; }

        /// <summary>
        /// Listing's property types (i.e. "Single Family").
        /// </summary>
        public required PropertyType[] PropertyTypes { get; set; }

        /// <summary>
        /// Radius of search in miles.
        /// </summary>
        public required int Radius { get; set; }

        /// <summary>
        /// Minimum number of bedrooms in listing (i.e. 4).
        /// </summary>
        public required double Bedrooms { get; set; }

        /// <summary>
        /// Maximum number of bathrooms in listing (i.e. 4).
        /// </summary>
        public required double Bathrooms { get; set; }

        /// <summary>
        /// Maximum price of listing (i.e. 5000).
        /// </summary>
        public required int MaxPrice { get; set; }

        /// <summary>
        /// Maximum number of days listing has been on market (i.e. 5).
        /// </summary>
        public required int DaysOnMarket { get; set; } = 1;

        /// <summary>
        /// Maximum number of days listing can be before removing from database (i.e. 30).
        /// </summary>
        public required int MaxDaysOnMarketToKeep { get; set; } = 30;
    }
}
