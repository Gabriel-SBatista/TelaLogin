using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login.Cross.Logging
{
    public static class LoggingConfiguration
    {
        public static void UseCustomLog(this IApplicationBuilder app, ILoggerFactory loggerFactory, IConfiguration configuration)
        {
            var newRelicSection = configuration.GetSection("NewRelic");
            var licenceKey = newRelicSection["LicenseKey"];

            var loggerConfiguration = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Information)
                .WriteTo.Console()
                    .WriteTo.NewRelicLogs(
                        applicationName: "login",
                        licenseKey: licenceKey
                    );

            Log.Logger = loggerConfiguration.CreateLogger();

            loggerFactory.AddSerilog(Log.Logger);
        }
    }
}
