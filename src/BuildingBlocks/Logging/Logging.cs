
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Logging;

public static class Logging
{
    public static Action<HostBuilderContext, LoggerConfiguration> Configure =>
      (context, configuration) =>
      {
          configuration
              .Enrich.FromLogContext()
              .Enrich.WithProperty("ContentRootPath", context.HostingEnvironment.ContentRootPath)
              .Enrich.WithProperty("ServiceName", context.Configuration["Serilog:Properties:ServiceName"] ?? "DefaultService")
              .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName)
              .WriteTo.Console(outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{ServiceName}/{Environment}] {Message:lj}{NewLine}{Exception}")
              .ReadFrom.Configuration(context.Configuration);
      };
}
