using Listings_Portal.Lib.Models.Cloud;
using System.ComponentModel.DataAnnotations;

namespace Listings_Portal.Lib.Models.Entities
{
    /// <summary> Realtor model. </summary>
    public class Realtor : RealtorCloud
    {
        /// <summary>
        /// Realtor ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets realtor model.
        /// </summary>
        /// <param name="cloudModel"> Cloud model. </param>
        /// <returns> Realtor DB model. </returns>
        public static Realtor? FromCloud(RealtorCloud? cloudModel)
        {
            return cloudModel == null ? null : new Realtor()
            {
                Name = cloudModel.Name,
                Phone = cloudModel.Phone,
                Email = cloudModel.Email,
                Website = cloudModel.Website,
            };
        }
    }
}
