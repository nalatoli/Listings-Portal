using Listings_Portal.Lib.Models.Api;
using Listings_Portal.Lib.Models.Api.Dtos;
using Listings_Portal.Lib.Models.Entities;
using Listings_Portal.Lib.Tools.Cloud.RentCast;
using Listings_Portal.Tools;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Listings_Portal.Controllers
{
    /// <summary> 
    /// Sale/rental listings controller. 
    /// </summary>
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class ListingsController(ListingsDbContext dbContext) : ControllerBase
    {
        /// <summary>
        /// Search rental listings.
        /// </summary>
        /// <param name="latitude"> Latitude of search. </param>
        /// <param name="longitude"> Longitude of search. </param>
        /// <param name="radius"> Radius of search (miles). </param>
        /// <param name="type"> Listing type ("rent" or "sale"). </param>
        /// <param name="daysOld"> Number of days since listing was posted (0 for no filter). </param>
        /// <param name="bedrooms"> Minimum number of bedrooms (0 for no filter). </param>
        /// <param name="bathrooms"> Minimum number of bathrooms (0 for no filter). </param>
        /// <param name="page"> Page number to search (>=1). </param>
        /// <param name="pageSize"> Number of items per page (1 -> 50). </param>
        /// <returns> Pageinated rental listings. </returns>
        [HttpGet]
        public async Task<ActionResult<PagedResponse<ListingDto>>> GetAsync(
            [FromQuery, BindRequired] double latitude = 40.903168,
            [FromQuery, BindRequired] double longitude = -73.864464,
            [FromQuery, BindRequired] double radius = 1,
            [FromQuery] ListingType type = ListingType.Rent,
            [FromQuery] int daysOld = 0,
            [FromQuery] float bedrooms = 0,
            [FromQuery] float bathrooms = 0,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            page = Math.Max(page, 1);
            pageSize = Math.Clamp(pageSize, 1, 50);
            var earliestDate = DateTime.UtcNow.AddDays(-daysOld);
            var location = Listing.GetPoint(longitude, latitude);
            var radiusMeters = radius * 1609.344;

            var query = dbContext.Listings.Where(l =>
                l.Type == type &&
                (bedrooms <= 0 || l.Bedrooms >= bedrooms) &&
                (bathrooms <= 0 || l.Bathrooms >= bedrooms) &&
                (daysOld <= 0 || l.ListedDate >= earliestDate) &&
                (radius <= 0 || EF.Functions.IsWithinDistance(l.Location, location, radiusMeters, false)));

            var items = await query
                .OrderBy(l => l.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .Select(l => new ListingDto(
                    l.Id,
                    l.Guid,
                    l.AddressLine1,
                    l.AddressLine2,
                    l.City,
                    l.State,
                    l.ZipCode,
                    l.County,
                    l.Location.X,
                    l.Location.Y,
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
                    l.DaysOnMarket,
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
                    ))
                .ToListAsync();

            items.ForEach(i => Debug.WriteLine(i.ListingAgent));

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
        [HttpGet("{id}")]
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
    }
}
