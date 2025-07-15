using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Listings_Portal.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "Hoa",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fee = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hoa", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Realtor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    Email = table.Column<string>(type: "text", nullable: true),
                    Website = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Realtor", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Listings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Guid = table.Column<string>(type: "text", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false),
                    AddressLine1 = table.Column<string>(type: "text", nullable: false),
                    AddressLine2 = table.Column<string>(type: "text", nullable: true),
                    City = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<string>(type: "text", nullable: false),
                    ZipCode = table.Column<string>(type: "text", nullable: false),
                    County = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<Point>(type: "geometry (Point, 4326)", nullable: false),
                    PropertyType = table.Column<string>(type: "text", nullable: false),
                    Bedrooms = table.Column<double>(type: "double precision", nullable: false),
                    Bathrooms = table.Column<double>(type: "double precision", nullable: false),
                    SquareFootage = table.Column<int>(type: "integer", nullable: false),
                    LotSize = table.Column<int>(type: "integer", nullable: false),
                    YearBuilt = table.Column<int>(type: "integer", nullable: false),
                    HoaId = table.Column<int>(type: "integer", nullable: true),
                    Status = table.Column<string>(type: "text", nullable: false),
                    Price = table.Column<long>(type: "bigint", nullable: false),
                    ListingType = table.Column<string>(type: "text", nullable: false),
                    ListedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DaysOnMarket = table.Column<long>(type: "bigint", nullable: false),
                    MlsName = table.Column<string>(type: "text", nullable: true),
                    MlsNumber = table.Column<string>(type: "text", nullable: true),
                    ListingAgentId = table.Column<int>(type: "integer", nullable: true),
                    ListingOfficeId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Listings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Listings_Hoa_HoaId",
                        column: x => x.HoaId,
                        principalTable: "Hoa",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Listings_Realtor_ListingAgentId",
                        column: x => x.ListingAgentId,
                        principalTable: "Realtor",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Listings_Realtor_ListingOfficeId",
                        column: x => x.ListingOfficeId,
                        principalTable: "Realtor",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Listings_HoaId",
                table: "Listings",
                column: "HoaId");

            migrationBuilder.CreateIndex(
                name: "IX_Listings_ListingAgentId",
                table: "Listings",
                column: "ListingAgentId");

            migrationBuilder.CreateIndex(
                name: "IX_Listings_ListingOfficeId",
                table: "Listings",
                column: "ListingOfficeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Listings");

            migrationBuilder.DropTable(
                name: "Hoa");

            migrationBuilder.DropTable(
                name: "Realtor");
        }
    }
}
