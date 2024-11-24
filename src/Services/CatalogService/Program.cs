using Asp.Versioning;
using CatalogService.Exceptions;
using CatalogService.Helpers;
using CatalogService.Models;
using CatalogService.Repository;
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

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
    configuration.ReadFrom.Services(services);
});

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();


// Versioning
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

// Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    var redisSettings = builder.Configuration.GetSection("Redis").Get<RedisSettings>();
    options.Configuration = redisSettings.Configuration;
    options.InstanceName = redisSettings.InstanceName;
});


// Add services to the container.
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

builder.Services.AddSingleton<IProductRepository, ProductRepository>();

builder.Services.AddScoped<ProductsService>();

builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    }).AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    });

// Health
builder.Services.AddHealthChecks()
    .AddMongoDb(
        mongodbConnectionString: builder.Configuration.GetSection("MongoDB").Get<CatalogDBSettings>().ConnectionString,
        name: "Database",
        failureStatus: HealthStatus.Unhealthy,
        tags: new string[] { "catalogdb" });

builder.Services.AddHealthChecks().AddRedis(
    redisConnectionString: builder.Configuration.GetSection("Redis").Get<RedisSettings>().Configuration,
    name: "Redis",
    failureStatus: HealthStatus.Unhealthy,
    tags: new string[] { "redis", "cach" }
    );

var healthCheckUISection = builder.Configuration.GetSection("HealthChecksUI:Endpoints:CatalogService");
string healthCheckName = healthCheckUISection.GetValue<string>("Name");
string healthCheckUrl = healthCheckUISection.GetValue<string>("Url");


builder.Services.AddHealthChecksUI(
    options =>
    {
        options.SetEvaluationTimeInSeconds(10);
        options.SetMinimumSecondsBetweenFailureNotifications(60);
        options.MaximumHistoryEntriesPerEndpoint(50);
        options.AddHealthCheckEndpoint(healthCheckName, healthCheckUrl);
    }
    ).AddInMemoryStorage();




//Validation
builder.Services.AddValidatorsFromAssemblyContaining<ProductRequest.Create.Validator>();
builder.Services.AddValidatorsFromAssemblyContaining<ProductRequest.Index.Validator>();

builder.Services.AddFluentValidationAutoValidation();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();




var app = builder.Build();


// Configure the HTTP request pipeline.
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

app.UseHealthChecksUI(config => config.UIPath = "/health-ui");

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();


