using Asp.Versioning;
using HealthChecks.UI.Client;
using InventoryService.Data;
using InventoryService.Repositories;
using InventoryService.Services;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------------
// Logging
// ------------------------------------
builder.Host.UseSerilog(Logging.Logging.Configure);

// ------------------------------------
// Global Exception Handling
// ------------------------------------
//builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
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
// DB Configuration
// ------------------------------------
builder.Services.AddDbContext<InventoryDbContext>(
    options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("Database"))

    );

// ------------------------------------
// Repositories and Services
// ------------------------------------
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductsService, ProductsService>();

// ------------------------------------
// AutoMapper
// ------------------------------------
builder.Services.AddAutoMapper(typeof(Program));

// ------------------------------------
// MVC and Model Validation
// ------------------------------------
builder.Services.AddControllers();

// ------------------------------------
// Health Checks
// ------------------------------------
builder.Services.AddHealthChecks()
    .AddCheck(
        name: "Self",
        check: () => HealthCheckResult.Healthy(),
        tags: new[] { "catalog_service" }
    )
    .AddNpgSql(
        connectionString: builder.Configuration.GetConnectionString("Database"),
        name: "Database",
        failureStatus: HealthStatus.Unhealthy,
        tags: new[] { "inventorydb" }
    );



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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// ------------------------------------
// Run Application
// ------------------------------------
app.Run();
