using Listings_Portal.Lib.Models.Api.Dtos;
using Listings_Portal.Lib.Models.Api;
using Listings_Portal.Tests.Tools;
using Listings_Portal.Tools;
using System.Net.Http.Json;
using Microsoft.Extensions.DependencyInjection;
using FluentAssertions;
using Listings_Portal.Tests.Tools.Extensions;
using Microsoft.EntityFrameworkCore;
using Listings_Portal.Lib.Tools.Cloud.RentCast;
using NetTopologySuite.Geometries;

namespace Listings_Portal.Tests.Integration.Controllers
{
    public sealed class ListingsControllerTests : IClassFixture<PostgresContainerFixture>, IDisposable
    {
        private readonly TestAppFactory factory;
        private readonly IServiceScope scope;
        private readonly HttpClient client;
        private readonly ListingsDbContext dbContext;

        public ListingsControllerTests(PostgresContainerFixture fixture)
        {   
            string conString = PostgreExt.AddDatabaseAsync(fixture.Container).Result;
            factory = new TestAppFactory(conString);
            scope = factory.Services.CreateScope();
            dbContext = scope.ServiceProvider.GetRequiredService<ListingsDbContext>();
            dbContext.Database.Migrate();
            client = factory.CreateClient();
        }

        public void Dispose()
        {
            factory.Dispose();
            scope.Dispose();
            client.Dispose();
        }

        private static string GetUrl(string endpoint)
            => $"/api/v1/listings{endpoint}";


        [Fact]
        public async Task Gets_Only_Rent_Listings_Within_Radius()
        {
            const double radius = 5;
            double radiusDeg = radius * 0.0145;
            double lat = SampleData.DefaultLatDeg;
            double lon = SampleData.DefaultLonDeg;

            var insideRent = SampleData.CreateListingFaker(ListingType.Rent, lat, lon, 5).Generate(20);
            var outsideRent = SampleData.CreateListingFaker(ListingType.Rent, lat, lon, 20).Generate(40)
                .Where(l =>
                    Math.Abs(l.Location.X - lon) > radiusDeg &&
                    Math.Abs(l.Location.Y - lat) > radiusDeg)
                .ToList();
            var insideSale = SampleData.CreateListingFaker(ListingType.Sale, lat, lon, 5).Generate(20);

            await dbContext.SeedAsync(insideRent, outsideRent, insideSale);
            var pee = dbContext.Listings.Select(r => EF.Functions.Distance(new Point(lon, lat), r.Location, false) / 1609.344).Where(d => d < 5).ToArray();

            var resp = await client.GetFromJsonAsync<PagedResponse<ListingDto>>(GetUrl($"?" +
                $"type={ListingType.Rent}&" +
                $"latitude={lat}&" +
                $"longitude={lon}&" +
                $"radius={radius}"));

            resp!.TotalCount.Should().Be(insideRent.Count);
            var expectedIds = insideRent.Select(i => i.Guid);
            var actualIds = resp.Items.Select(i => i.Guid);
            actualIds.Should().BeEquivalentTo(expectedIds);
        }
    }
}
