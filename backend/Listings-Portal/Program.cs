using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Listings_Portal.Middleware;
using Listings_Portal.Tools;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text.Json.Serialization;

namespace Listings_Portal;

#pragma warning disable CS1591
public partial class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        /* Add services */
        builder.Services.AddDbContext<ListingsDbContext>(options => options.UseNpgsql(
            Environment.GetEnvironmentVariable("LISTINGS_DB_CON"),
            x => x.UseNetTopologySuite()));

        /* Add all MVC controllers */
        builder.Services.AddRouting(options => options.LowercaseUrls = true);
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        /* Add API versioning with API explorer versioning support */
        builder.Services.AddApiVersioning(option =>
        {
            option.AssumeDefaultVersionWhenUnspecified = true;
            option.DefaultApiVersion = new ApiVersion(1);
            option.ReportApiVersions = true;
            option.ApiVersionReader = new UrlSegmentApiVersionReader();
        }).AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'V";
            options.SubstituteApiVersionInUrl = true;
        });

        /* Add endpoints to API explorer and add custom versioning swagger gen options */
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        builder.Services.AddSwaggerGen(c =>
        {
            /* Add XML documentation from code */
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        /* Build app */
        var app = builder.Build();

        /* Add swagger UI */
        if (app.Environment.IsDevelopment())
        {
            var apiVersionDescriptionProvider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }
            });
        }

        /* Add middleware */
        app.UseHttpsRedirection();

        /* Map all controllers and require the default authorization policy */
        app.MapControllers();

        /* Start app and wait until its shutdown */
        await app.RunAsync();
    }
}
#pragma warning restore CS1591

