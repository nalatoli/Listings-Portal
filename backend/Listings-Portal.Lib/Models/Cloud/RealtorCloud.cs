using System.ComponentModel.DataAnnotations;

namespace Listings_Portal.Lib.Models.Cloud
{
    /// <summary> Relator model. </summary>
    public class RealtorCloud
    {
        /// <summary>
        /// Realtor's name (i.e. "Zachary Barton").
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Realtor's phone number (i.e. "5129948203").
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Realtor's email (i.e. "zak-barton@realtytexas.co").
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Realtor's wesbite (i.e. "https://zak-barton.realtytexas.homes").
        /// </summary>
        public string? Website { get; set; }
    }
}
