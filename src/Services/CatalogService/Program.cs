using Asp.Versioning;
using CatalogService.Exceptions;
using CatalogService.Filters;
using CatalogService.Helpers;
using CatalogService.Models;
using CatalogService.Repositories;
using CatalogService.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------
// Logging
// ------------------------------------
builder.Host.UseSerilog(Logging.Logging.Configure);

// ------------------------------------
// Global Exception Handling
// ------------------------------------
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// ------------------------------------
// API Versioning
// ------------------------------------
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"));
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});

// ------------------------------------
// MongoDB Configuration
// ------------------------------------
builder.Services.Configure<CatalogDBSettings>(builder.Configuration.GetSection("MongoDB"));
builder.Services.AddSingleton<IMongoClient>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<CatalogDBSettings>>().Value;
    return new MongoClient(settings.ConnectionString);
});
builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<CatalogDBSettings>>().Value;
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings.DatabaseName);
});

// ------------------------------------
// Repositories and Services
// ------------------------------------
builder.Services.AddSingleton<IProductRepository, ProductRepository>();
builder.Services.AddSingleton<IProductsService, ProductsService>();

// ------------------------------------
// AutoMapper
// ------------------------------------
builder.Services.AddAutoMapper(typeof(Program));

// ------------------------------------
// MVC and Model Validation
// ------------------------------------
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ModelValidationAttribute>();
})
.ConfigureApiBehaviorOptions(options =>
{
    options.SuppressModelStateInvalidFilter = true;
})
.AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
});

// ------------------------------------
// Health Checks
// ------------------------------------
builder.Services.AddHealthChecks()
    .AddCheck(
        name: "Self",
        check: () => HealthCheckResult.Healthy(),
        tags: new[] { "catalog_service" }
    )
    .AddMongoDb(
        mongodbConnectionString: builder.Configuration.GetSection("MongoDB").Get<CatalogDBSettings>().ConnectionString,
        name: "Database",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "catalogdb" }
    );

// ------------------------------------
// Validation
// ------------------------------------
builder.Services.AddValidatorsFromAssemblyContaining<ProductRequest.Create.Validator>();
builder.Services.AddValidatorsFromAssemblyContaining<ProductRequest.Index.Validator>();
builder.Services.AddFluentValidationAutoValidation();

// ------------------------------------
// Swagger
// ------------------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ------------------------------------
// Build Application
// ------------------------------------
var app = builder.Build();

// ------------------------------------
// Middleware Pipeline
// ------------------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseSerilogRequestLogging();
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// ------------------------------------
// Run Application
// ------------------------------------
app.Run();
