using System.ComponentModel.DataAnnotations;

namespace Listings_Portal.Lib.Models.Api
{
    /// <summary>
    /// Wrapper for paged response.
    /// </summary>
    /// <typeparam name="T"> Item's type. </typeparam>
    public class PagedResponse<T> where T : class
    {
        /// <summary>
        /// Current page.
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Number of items in page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of items.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Items in response.
        /// </summary>
        [Required]
        public required IEnumerable<T> Items { get; set; }
    }
}
