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
        /// <param name="daysOld"> Maximum number of days listing was on market (&lt;1 means any). </param>
        /// <param name="cancellationToken"> Optional cancellation token. </param>
        /// <returns> Available rent listings. </returns>
        public static async Task<Listing[]> GetRentListingsAsync(
            PropertyType propertyType,
            double latitude,
            double longitude,
            double radius,
            int daysOld = 0,
            CancellationToken cancellationToken = default)
        {
            using var client = new RentCastClient();
            return await client.Get<Listing[]>(
                "listings/rental/long-term",
                $"limit=200&status=Active" +
                $"&propertyType={propertyType.GetDescription()}" +
                $"&latitude={latitude}" +
                $"&longitude={longitude}" +
                $"&radius={radius}" +
                (daysOld >= 1 ? $"&daysOld={daysOld}" : ""),
                cancellationToken);
        }
    }
}
