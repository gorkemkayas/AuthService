using Serilog;

namespace AuthService.API.Extensions
{
    public static class LoggingExtensions
    {
        public static WebApplicationBuilder AddCustomLogging(this WebApplicationBuilder builder)
        {
            var loggerConfig = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console();

            var seqUrl = builder.Configuration["Serilog:SeqUrl"];
            if (!string.IsNullOrWhiteSpace(seqUrl))
            {
                loggerConfig = loggerConfig.WriteTo.Seq(seqUrl);
            }

            Log.Logger = loggerConfig.CreateLogger();

            builder.Host.UseSerilog();

            return builder;
        }
    }
}
