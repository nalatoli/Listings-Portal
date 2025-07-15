using System.ComponentModel.DataAnnotations;

namespace Listings_Portal.Lib.Models.Cloud
{
    /// <summary> HOA cloud model. </summary>
    public class HoaCloud
    {
        /// <summary>
        /// HOA fee.
        /// </summary>
        public required int Fee { get; set; }
    }
}
