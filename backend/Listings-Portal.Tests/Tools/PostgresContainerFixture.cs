using Listings_Portal.Tests.Tools.Extensions;
using Testcontainers.PostgreSql;

namespace Listings_Portal.Tests.Tools
{
    public class PostgresContainerFixture : IAsyncLifetime
    {
        /// <summary>
        /// Running Postgre test container.
        /// </summary>
        public PostgreSqlContainer Container { get; private set; } = null!;

        public async Task InitializeAsync()
        {
            Container = PostgreExt.GetTestContainer();
            await Container.StartAsync();
        }

        public async Task DisposeAsync()
        {
            await Container.DisposeAsync();
        }
    }
}