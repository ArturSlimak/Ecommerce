
using Elastic.Serilog.Sinks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Logging;

public static class Logging
{
    private static readonly string outputTemplate = "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{ServiceName}/{Environment}] {Message:lj}{NewLine}{Exception}";
    public static Action<HostBuilderContext, LoggerConfiguration> Configure =>
      (context, configuration) =>
      {
          var elasticUri = context.Configuration.GetValue<string>("ElasticConfiguration:Uri");
          configuration
              .Enrich.FromLogContext()
              .Enrich.WithProperty("ContentRootPath", context.HostingEnvironment.ContentRootPath)
              .Enrich.WithProperty("ServiceName", context.Configuration["Serilog:Properties:ServiceName"] ?? "DefaultService")
              .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
              .WriteTo.Console(outputTemplate: outputTemplate)
              .WriteTo.Elasticsearch(new[] { new Uri(elasticUri) }, opts =>
              {

              })
              .ReadFrom.Configuration(context.Configuration);
      };
}
