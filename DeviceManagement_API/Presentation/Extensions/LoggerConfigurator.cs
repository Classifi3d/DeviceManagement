using Serilog;
using Serilog.Core;
using Serilog.Events;

namespace Presentation.Extensions;

public static class LoggerConfigurator
{
    public static Logger ConfigureLogger(IConfiguration configuration)
    {
        var serilogConfig = new LoggerConfiguration();
        serilogConfig
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning);

        serilogConfig
            .Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .Enrich.WithEnvironmentName()
            .Enrich.FromLogContext();

        serilogConfig
            .WriteTo.Console();

        return serilogConfig.CreateLogger();
    }

}
