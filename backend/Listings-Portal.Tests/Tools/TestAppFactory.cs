using Listings_Portal.Tests.Tools.Extensions;
using Listings_Portal.Tools;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Testcontainers.PostgreSql;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;

namespace Listings_Portal.Tests.Tools
{
    public class TestAppFactory(string connectionString) : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            Bogus.Randomizer.Seed = new Random(1234);
            builder.ConfigureServices(services =>
            {
                services.RemoveAll<IHostedService>();
                services.RemoveAll<DbContextOptions<ListingsDbContext>>();
                services.AddDbContext<ListingsDbContext>(options =>
                    options
                    .UseNpgsql(connectionString, o => o.UseNetTopologySuite()));
            });
            builder.ConfigureLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddDebug();
                logging.AddConsole();
            });
        }
    }

}