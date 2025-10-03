using Listings_Portal.Lib.Models.Cloud;
using Listings_Portal.Lib.Models.Entities;
using Listings_Portal.Lib.Tools.Extensions;

namespace Listings_Portal.Lib.Tools.Cloud.RentCast
{
    /// <summary>
    /// RentCast API.
    /// </summary>
    public static class RentCast
    {

        /// <summary>
        /// Gets rent listings from specified location and property type.
        /// </summary>
        /// <param name="propertyType"> Property type. </param>
        /// <param name="latitude"> Center of search latitude. </param>
        /// <param name="longitude"> Center of search longitude. </param>
        /// <param name="radius"> Radius of search in miles (max 100). </param>
        /// <param name="bedrooms"> Minimum number of bedrooms (0 for studio). </param>
        /// <param name="bathrooms"> Minimum number of bathrooms (0 for studio). </param>
        /// <param name="daysOld"> Maximum number of days listing was on market (&lt;1 means any). </param>
        /// <param name="cancellationToken"> Optional cancellation token. </param>
        /// <returns> Available rent listings. </returns>
        public static async Task<ListingCloud[]> GetRentListingsAsync(
            IEnumerable<PropertyType> propertyTypes,
            double latitude,
            double longitude,
            double radius,
            double bedrooms,
            double bathrooms,
            int daysOld = 0,
            CancellationToken cancellationToken = default)
        {
            using var client = new RentCastClient();
            return await client.Get<ListingCloud[]>(
                "listings/rental/long-term",
                $"limit=500&status=Active" +
                $"&propertyType={string.Join('|', propertyTypes.Select(p => p.GetDescription()))}" +
                $"&latitude={latitude}" +
                $"&longitude={longitude}" +
                $"&bedrooms={bedrooms}" +
                $"&bathrooms={bathrooms}" +
                $"&radius={radius}" +
                (daysOld >= 1 ? $"&daysOld={daysOld}" : ""),
                cancellationToken);
        }
    }
}
