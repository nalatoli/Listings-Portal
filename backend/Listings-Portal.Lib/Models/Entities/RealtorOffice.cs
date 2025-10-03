using Listings_Portal.Lib.Models.Cloud;
using System.ComponentModel.DataAnnotations;

namespace Listings_Portal.Lib.Models.Entities
{
    /// <summary> Realtor office model. </summary>
    public class RealtorOffice : RealtorCloud
    {
        /// <summary>
        /// Parent listing ID.
        /// </summary>
        public int ListingId { get; set; }

        /// <summary>
        /// Parent listing.
        /// </summary>
        public Listing? Listing { get; set; }

        /// <summary>
        /// Gets realtor model.
        /// </summary>
        /// <param name="cloudModel"> Cloud model. </param>
        /// <returns> Realtor DB model. </returns>
        public static RealtorOffice? FromCloud(RealtorCloud? cloudModel)
        {
            return cloudModel == null ? null : new RealtorOffice()
            {
                Name = cloudModel.Name,
                Phone = cloudModel.Phone,
                Email = cloudModel.Email,
                Website = cloudModel.Website,
            };
        }
    }
}
