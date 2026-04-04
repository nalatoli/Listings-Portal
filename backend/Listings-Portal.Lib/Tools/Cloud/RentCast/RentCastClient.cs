using Listings_Portal.Lib.Models.Cloud;
using Listings_Portal.Lib.Tools.Cloud;
using Listings_Portal.Lib.Tools.Extensions;
using Microsoft.Extensions.Logging;

namespace Listings_Portal.Lib.Tools.Cloud.RentCast
{
    public class RentCastClient(HttpClient client, ILogger<RentCastClient> logger) : BaseClient(client, logger)
    {
        private const string keyFilename = "rckey.txt";

        public static string GetApiKey()
        {
            return GetApiKey(keyFilename);
        }

        public static void CreateApiKeyFile(string apiKey)
        {
            CreateApiKeyFile(apiKey, keyFilename);
        }

        /// <summary>
        /// Gets rent listings from specified location and property type.
        /// </summary>
        /// <param name="propertyTypes"> Property type. </param>
        /// <param name="latitude"> Center of search latitude. </param>
        /// <param name="longitude"> Center of search longitude. </param>
        /// <param name="radius"> Radius of search in miles (max 100). </param>
        /// <param name="bedrooms"> Minimum number of bedrooms (0 for studio). </param>
        /// <param name="bathrooms"> Minimum number of bathrooms (0 for studio). </param>
        /// <param name="maxPrice"> Maximum price of listing (-1 for none). </param>
        /// <param name="limit"> Maximum number of listings to return. </param>
        /// <param name="daysOld"> Maximum number of days listing was on market (&lt;1 means any). </param>
        /// <param name="cancellationToken"> Optional cancellation token. </param>
        /// <returns> Available rent listings. </returns>
        public async Task<ListingCloud[]> GetRentListingsAsync(
            IEnumerable<PropertyType> propertyTypes,
            double latitude,
            double longitude,
            double radius,
            double bedrooms,
            double bathrooms,
            int maxPrice,
            int limit=500,
            int daysOld = 0,
            CancellationToken cancellationToken = default)
        {
            return await Get<ListingCloud[]>(
                "listings/rental/long-term",
                $"limit={limit}&status=Active" +
                $"&propertyType={string.Join('|', propertyTypes.Select(p => p.GetDescription()))}" +
                $"&latitude={latitude}" +
                $"&longitude={longitude}" +
                $"&bedrooms={bedrooms}:*" +
                $"&bathrooms={bathrooms}:*" +
                (maxPrice > 0 ? $"&price=*:{maxPrice}" : "") +
                $"&radius={radius}" +
                (daysOld >= 1 ? $"&daysOld={daysOld}" : ""),
                cancellationToken);
        }
    }
}
