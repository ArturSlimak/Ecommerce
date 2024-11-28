using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

var environment = builder.Environment.EnvironmentName;

builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
                      .AddJsonFile($"ocelot.{environment}.json", optional: true, reloadOnChange: true);

builder.Services.AddOcelot(builder.Configuration);


// Rate Limit  - commented since there is unresloved conflict with new rate limiting in .NET 8 and the one from Ocelot
/*var rateLimitOptions = new RateLimitOptions();
builder.Configuration.GetSection(RateLimitOptions.RateLimit).Bind(rateLimitOptions);
builder.Services.AddRateLimiter(options =>
{
    var tokenBucketOptions = rateLimitOptions.TokenBucket;

    options.AddTokenBucketLimiter(policyName: tokenBucketOptions.PolicyName, limiterOptions =>
    {
        limiterOptions.TokenLimit = tokenBucketOptions.TokenLimit;
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        limiterOptions.QueueLimit = tokenBucketOptions.QueueLimit;
        limiterOptions.ReplenishmentPeriod = TimeSpan.FromSeconds(tokenBucketOptions.ReplenishmentPeriod);
        limiterOptions.TokensPerPeriod = tokenBucketOptions.TokensPerPeriod;
        limiterOptions.AutoReplenishment = tokenBucketOptions.AutoReplenishment;
    });
    options.RejectionStatusCode = 429;
});*/



// Add services to the container.
builder.Services.AddControllers();
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
//app.UseRateLimiter();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseOcelot().Wait();

app.Run();
