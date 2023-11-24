using Serilog;

namespace LoggingConfiguration;

public class LoggerConfig
{
    public static ILogger ConfigureLogger()
    {
        return new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File("log.txt")
            .CreateLogger();
    }
}