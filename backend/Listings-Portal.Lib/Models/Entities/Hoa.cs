using Listings_Portal.Lib.Models.Cloud;
using System.ComponentModel.DataAnnotations;

namespace Listings_Portal.Lib.Models.Entities
{
    /// <summary> HOA DB model. </summary>
    public class Hoa : HoaCloud
    {
        /// <summary>
        /// HOA ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets HOA model.
        /// </summary>
        /// <param name="cloudModel"> Cloud model. </param>
        /// <returns> HOA model. </returns>
        public static Hoa? FromCloud(HoaCloud? cloudModel)
        {
            return cloudModel == null ? null : new Hoa()
            {
                Fee = cloudModel.Fee,
            };
        }
    }
}
