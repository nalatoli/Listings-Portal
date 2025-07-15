using Listings_Portal.Lib.Models.Entities;
using Listings_Portal.Tools;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.ComponentModel;
using System.Diagnostics;
using System.Xml.Linq;
using Testcontainers.PostgreSql;

namespace Listings_Portal.Tests.Tools.Extensions
{
    /// <summary>
    /// Extension for PostgreSQL.
    /// </summary>
    internal static class PostgreExt
    {
        private const string templateDbName = "templateDb";

        /// <summary>
        /// Gets docker image of test Postgre database.
        /// Use `GetTestDbContext` after starting container to get a database context for this container.
        /// </summary>
        /// <returns> Docker image of test Postgre database. </returns>
        public static PostgreSqlContainer GetTestContainer()
        {
            const string path = "/var/lib/pg/data";
            return new PostgreSqlBuilder()
                .WithImage("postgis/postgis:16-3.4")
                .WithDatabase(templateDbName)
                .WithUsername("postgres")
                .WithPassword("postgres")
                .WithPortBinding(0, 5432)
                .WithTmpfsMount(path, DotNet.Testcontainers.Configurations.AccessMode.ReadWrite)
                .WithEnvironment("PGDATA", path)
            .Build();
        }

        /// <summary>
        /// Gets database context from connection string.
        /// </summary>
        /// <param name="connectionString"> Connection string to existing database. </param>
        /// <returns> Database context. </returns>
        public static ListingsDbContext GetDatabaseContext(string connectionString)
        {
            return new ListingsDbContext(new DbContextOptionsBuilder<ListingsDbContext>()
                .UseNpgsql(connectionString, o => o.UseNetTopologySuite())
                .Options);
        }

        /// <summary>
        /// Add's new database.
        /// </summary>
        /// <param name="container"> Running Postgre container. </param>
        /// returns> Database connection string. </returns>
        public static async Task<string> AddDatabaseAsync(PostgreSqlContainer container)
        {
            using var con = await CreateConnectionAsync(container);
            return await AddDatabaseAsync(container, con);
        }

        /// <summary>
        /// Add's new database.
        /// </summary>
        /// <param name="container"> Running Postgre container. </param>
        /// <param name="con"> Opened connection to Postgre default database. </param>
        /// <returns> Database connection string. </returns>
        public static async Task<string> AddDatabaseAsync(PostgreSqlContainer container, NpgsqlConnection con)
        {
            var dbName = $"testdb_{Guid.NewGuid()}";
            await CreateDatabaseAsync(con, dbName);

            var builder = new NpgsqlConnectionStringBuilder(container.GetConnectionString())
            {
                Database = dbName
            };

            return builder.ConnectionString;
        }

        /// <summary>
        /// Seeds database context with specified records.
        /// </summary>
        /// <typeparam name="T"> Record type. </typeparam>
        /// <param name="dbContext"> Database context. </param>
        /// <param name="records"> Records to add. </param>
        /// <returns> Number of records written to database.</returns>
        public static async Task<int> SeedAsync<T>(this DbContext dbContext, params IEnumerable<T>[] records) where T : class
        {
            await dbContext.AddRangeAsync(records.SelectMany(r => r));
            return await dbContext.SaveChangesAsync();       
        }

        /// <summary>
        /// Creates connection to Postgre container's default database.
        /// </summary>
        /// <param name="container"> Postgre container. </param>
        /// <returns> Creates connection to Postgre container's default database. </returns>
        public static async Task<NpgsqlConnection> CreateConnectionAsync(PostgreSqlContainer container)
        {
            var con = new NpgsqlConnection(container.GetConnectionString());
            await con.OpenAsync();
            return con;
        }

        private static async Task<NpgsqlConnection> CreateDatabaseAsync(NpgsqlConnection con, string name)
        {
            using var cmd = con.CreateCommand();
            cmd.CommandText = $"CREATE DATABASE \"{name}\" TEMPLATE  \"{templateDbName}\";";
            await cmd.ExecuteNonQueryAsync();
            return con;
        }
    }
}
