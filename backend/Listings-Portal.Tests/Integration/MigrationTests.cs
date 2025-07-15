using FluentAssertions;
using Listings_Portal.Tests.Tools;
using Listings_Portal.Tests.Tools.Extensions;
using Listings_Portal.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Testcontainers.PostgreSql;

namespace Listings_Portal.Tests.Integration
{
    public sealed class MigrationTests : IClassFixture<PostgresContainerFixture>, IDisposable
    {
        private readonly ListingsDbContext dbContext;

        public MigrationTests(PostgresContainerFixture fixture)
        {
            dbContext = PostgreExt.GetDatabaseContext(PostgreExt.AddDatabaseAsync(fixture.Container).Result);
            dbContext.Database.Migrate();
        }

        public void Dispose()
        {
            dbContext.Dispose();
        }

        [Fact]
        public async Task Migrations_Should_Apply_On_Fresh_Database()
        {
            (await dbContext.Database.CanConnectAsync()).Should().BeTrue();
        }

        [Fact]
        public async Task Should_Insert_And_Read_After_Migration()
        {
            var data = SampleData.CreateListingFaker().Generate(1).First();
            await dbContext.SeedAsync([data]);
            var result = await dbContext.Listings.FindAsync(data.Id);
            result.Should().BeEquivalentTo(data);
        }

        [Fact]
        public async Task ListingsTable_Should_Have_Expected_Columns()
        {
            using var command = dbContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = @"
                SELECT column_name
                FROM information_schema.columns
                WHERE table_schema = 'public'
                  AND table_name = 'Listings';";

            using var reader = await command.ExecuteReaderAsync();
            var columns = new List<string>();
            while (await reader.ReadAsync())
                columns.Add(reader.GetString(0));
            
            columns.Should().Contain(["Id", "AddressLine1", "City", "State", "ZipCode"]);
        }
    }
}
