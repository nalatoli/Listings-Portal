using Hangfire;
using Listings_Portal.Lib.Models.Api;
using Listings_Portal.Lib.Models.Cloud;
using Listings_Portal.Lib.Models.Entities;
using Listings_Portal.Lib.Tools.Cloud.RentCast;
using Listings_Portal.Tools;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.Text.Json;

namespace Listings_Portal.BackgroundServices
{
    internal class PullBackgroundService(ILogger<PullBackgroundService> logger, IServiceProvider service, IOptionsMonitor<SearchOptions> searchOptions)
    {
        private readonly ILogger<PullBackgroundService> logger = logger;
        private readonly IServiceProvider service = service;
        private SearchOptions Options => searchOptions.CurrentValue;
        private readonly JsonSerializerOptions serializeOptions = new JsonSerializerOptions() { WriteIndented = true };

        public async Task RunAsync(IJobCancellationToken jct)
        {
            var now = DateTime.UtcNow;
            logger.LogInformation("{DT}: Finding listings with options:\n{options}", now, JsonSerializer.Serialize(Options, serializeOptions));

#if DEBUG
            await Task.CompletedTask;

#else

            var newCloudListings = await RentCast.GetRentListingsAsync(
                propertyTypes: Options.PropertyTypes,
                latitude: Options.Latitude,
                longitude: Options.Longitude,
                bedrooms: Options.Bedrooms,
                bathrooms: Options.Bathrooms,
                maxPrice: Options.MaxPrice,
                radius: Options.Radius,
                daysOld: Options.DaysOnMarket,
                cancellationToken: jct.ShutdownToken);
            logger.LogInformation("--> Fetched {count} new cloud listings", newCloudListings.Length);

            using var scope = service.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ListingsDbContext>();

            newCloudListings = [.. newCloudListings.GroupBy(l => l.Id).Select(g => g.First())];
            var existingDbListings = db.Listings.ToDictionary(x => x.Guid);

            foreach (var newListing in newCloudListings)
            {
                var newDbListing = Listing.FromCloud(newListing);
                if (!existingDbListings.TryGetValue(newDbListing.Guid, out var existingDbListing))
                {
                    db.Add(newDbListing);
                }

                else
                {
                    newDbListing.Id = existingDbListing.Id;
                    db.Entry(existingDbListing).CurrentValues.SetValues(newDbListing);
                }
            }

            var maxTimespan = TimeSpan.FromDays(Options.MaxDaysOnMarketToKeep);
            var deleteListings = from row in db.Listings where now - row.ListedDate > maxTimespan select row;
            db.RemoveRange(deleteListings);
            logger.LogInformation("--> State entries written to database: {count}", await db.SaveChangesAsync(jct.ShutdownToken));

#endif
        }
    }
}
