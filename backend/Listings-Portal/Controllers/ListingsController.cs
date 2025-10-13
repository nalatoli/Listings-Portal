using Listings_Portal.Lib.Models.Api;
using Listings_Portal.Lib.Models.Api.Dtos;
using Listings_Portal.Lib.Models.Entities;
using Listings_Portal.Lib.Tools.Cloud.RentCast;
using Listings_Portal.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Listings_Portal.Controllers
{
    /// <summary> 
    /// Sale/rental listings controller. 
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ListingsController(ListingsDbContext dbContext) : ControllerBase
    {
        private readonly TimeZoneInfo nyTz = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");

        /// <summary>
        /// Search rental listings by counties.
        /// </summary>
        /// <param name="counties"> Counties to look in (Delimited by '|'). </param>
        /// <param name="type"> Listing type ("rent" or "sale"). </param>
        /// <param name="daysOld"> Number of days since listing was posted (0 for no filter). </param>
        /// <param name="bedrooms"> Minimum number of bedrooms (0 for no filter). </param>
        /// <param name="bathrooms"> Minimum number of bathrooms (0 for no filter). </param>
        /// <param name="squareFootage"> Minimum square footage (0 for no filter). </param>
        /// <param name="yearBuilt"> Earliest year built (0 for no filter). </param>
        /// <param name="minPrice"> Minimum price (-1 means no filter). </param>
        /// <param name="maxPrice"> Maximum price (-1 means no filter). </param>
        /// <param name="page"> Page number to search (>=1). </param>
        /// <param name="pageSize"> Number of items per page (1 -> 50). </param>
        /// <returns> Pageinated rental listings. </returns>
        [HttpGet("counties")]
        public async Task<ActionResult<PagedResponse<ListingDto>>> GetByCountiesAsync(
            [FromQuery] string[]? counties,
            [FromQuery] ListingType type = ListingType.Rent,
            [FromQuery] int daysOld = 0,
            [FromQuery] float bedrooms = 0,
            [FromQuery] float bathrooms = 0,
            [FromQuery] float yearBuilt = 0,
            [FromQuery] float squareFootage = 0,
            [FromQuery] int minPrice = -1,
            [FromQuery] int maxPrice = -1,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 50);
            var earliestDate = TimeZoneInfo.ConvertTimeToUtc(TimeZoneInfo.ConvertTime(DateTime.UtcNow, nyTz).Date.AddDays(-daysOld), nyTz);

            var query = dbContext.Listings.Where(l =>
                l.Type == type &&
                (counties == null || counties.Length == 0 || counties.Contains(l.County)) &&
                (bedrooms <= 0 || l.Bedrooms >= bedrooms) &&
                (bathrooms <= 0 || l.Bathrooms >= bathrooms) &&
                (daysOld <= 0 || l.ListedDate >= earliestDate) &&
                (yearBuilt == 0 || l.YearBuilt >= yearBuilt) &&
                (squareFootage == 0 || l.SquareFootage >= squareFootage) &&
                (minPrice == -1 || l.Price >= minPrice) &&
                (maxPrice == -1 || l.Price <= maxPrice));

            var items = await query
                .OrderBy(l => l.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .Select(l => GetDtoListing(l))
                .ToListAsync();

            return new PagedResponse<ListingDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = await query.CountAsync()
            };
        }

        /// <summary>
        /// Search rental listings by range.
        /// </summary>
        /// <param name="latitude"> Latitude of search. </param>
        /// <param name="longitude"> Longitude of search. </param>
        /// <param name="radius"> Radius of search (miles). </param>
        /// <param name="type"> Listing type ("rent" or "sale"). </param>
        /// <param name="daysOld"> Number of days since listing was posted (0 for no filter). </param>
        /// <param name="bedrooms"> Minimum number of bedrooms (0 for no filter). </param>
        /// <param name="bathrooms"> Minimum number of bathrooms (0 for no filter). </param>
        /// <param name="squareFootage"> Minimum square footage (0 for no filter). </param>
        /// <param name="yearBuilt"> Earliest year built (0 for no filter). </param>
        /// <param name="minPrice"> Minimum price (-1 means no filter). </param>
        /// <param name="maxPrice"> Maximum price (-1 means no filter). </param>
        /// <param name="page"> Page number to search (>=1). </param>
        /// <param name="pageSize"> Number of items per page (1 -> 50). </param>
        /// <returns> Pageinated rental listings. </returns>
        [HttpGet("range")]
        public async Task<ActionResult<PagedResponse<ListingDto>>> GetByRangeAsync(
            [FromQuery, BindRequired] double latitude = 40.903168,
            [FromQuery, BindRequired] double longitude = -73.864464,
            [FromQuery, BindRequired] double radius = 1,
            [FromQuery] ListingType type = ListingType.Rent,
            [FromQuery] int daysOld = 0,
            [FromQuery] float bedrooms = 0,
            [FromQuery] float bathrooms = 0,
            [FromQuery] float yearBuilt = 0,
            [FromQuery] float squareFootage = 0,
            [FromQuery] int minPrice = -1,
            [FromQuery] int maxPrice = -1,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 50);
            var earliestDate = TimeZoneInfo.ConvertTimeToUtc(TimeZoneInfo.ConvertTime(DateTime.UtcNow, nyTz).Date.AddDays(-daysOld), nyTz);
            var location = Listing.GetPoint(longitude, latitude);
            var radiusMeters = radius * 1609.344;

            var query = dbContext.Listings.Where(l =>
                l.Type == type &&
                (bedrooms <= 0 || l.Bedrooms >= bedrooms) &&
                (bathrooms <= 0 || l.Bathrooms >= bathrooms) &&
                (daysOld <= 0 || l.ListedDate >= earliestDate) &&
                (yearBuilt == 0 || l.YearBuilt >= yearBuilt) &&
                (squareFootage == 0 || l.SquareFootage >= squareFootage) &&
                (minPrice == -1 || l.Price >= minPrice) &&
                (maxPrice == -1 || l.Price <= maxPrice) &&
                (radius <= 0 || EF.Functions.IsWithinDistance(l.Location, location, radiusMeters, false)));

            var items = await query
                .OrderBy(l => l.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .Select(l => GetDtoListing(l))
                .ToListAsync();

            var poo = DateTime.UtcNow;

            return new PagedResponse<ListingDto>
            {
                Items = items,
                Page = page,
                PageSize = pageSize,
                TotalCount = await query.CountAsync()
            };
        }

        /// <summary>
        /// Gets listing by ID.
        /// </summary>
        /// <param name="id"> Listing ID (i.e. "23"). </param>
        /// <returns> Listing by ID. </returns>
        [HttpGet("single/{id}")]
        [ProducesResponseType<Listing>(StatusCodes.Status200OK)]
        [ProducesResponseType<MessageResponse>(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ListingDto>> GetByIdAsync(int id)
        {
            var data = await dbContext.Listings
                .Include(l => l.Hoa)
                .Include(l => l.ListingAgent)
                .Include(l => l.ListingOffice)
                .FirstOrDefaultAsync(l => l.Id == id)
                ;
            return data == null ?
                NotFound(MessageResponse.Create($"Unable to find listing with ID '{id}'")) :
                ListingDto.FromEntity(data);
        }

        private static ListingDto GetDtoListing(Listing l)
        {
            return new ListingDto(
                   l.Id,
                   l.Guid,
                   l.AddressLine1,
                   l.AddressLine2,
                   l.City,
                   l.State,
                   l.ZipCode,
                   l.County,
                   l.Location.Y,
                   l.Location.X,
                   l.PropertyType,
                   l.Bedrooms,
                   l.Bathrooms,
                   l.SquareFootage,
                   l.LotSize,
                   l.YearBuilt,
                   l.Hoa == null ? null : new HoaDto(
                       l.Hoa.Fee),
                   l.Price,
                   l.ListingType,
                   l.ListedDate,
                   l.MlsName,
                   l.MlsNumber,
                   l.ListingAgent == null ? null : new RealtorDto(
                       l.ListingAgent.Name,
                       l.ListingAgent.Phone,
                       l.ListingAgent.Email,
                       l.ListingAgent.Website),
                  l.ListingOffice == null ? null : new RealtorDto(
                       l.ListingOffice.Name,
                       l.ListingOffice.Phone,
                       l.ListingOffice.Email,
                       l.ListingOffice.Website)
                   );
        }
    }
}
