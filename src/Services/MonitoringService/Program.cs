var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddHealthChecksUI(settings =>
{
    settings.SetEvaluationTimeInSeconds(10);
    settings.MaximumHistoryEntriesPerEndpoint(50);
}).AddInMemoryStorage();



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

app.MapHealthChecksUI(options =>
{
    options.UIPath = "/healthchecks-ui";
    options.ApiPath = "/healthchecks-api";
});

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
