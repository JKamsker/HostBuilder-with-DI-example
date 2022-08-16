using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

//args = new[] { "--Location:City", "abc" };
//args = new[] { "--Location__City", "abc" };


var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(configBuilder =>
    {
        var environmentName = Environment.GetEnvironmentVariable("Environment");

        configBuilder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
            .AddBetterCommandLine(args) // just a clone of AddCommandLine
            .AddEnvironmentVariables();
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.Configure<LocationConfiguration>(hostContext.Configuration.GetSection("Location"));
        services.AddTransient<LocationService>();
        services.AddTransient<EchoingService>();
    })
    .ConfigureLogging(x =>
    {
        x
            //.AddSimpleConsole(x =>
            //{
            //    x.SingleLine = true;
            //    x.ColorBehavior = LoggerColorBehavior.Enabled;
            //    x.IncludeScopes = true;
            //}).SetMinimumLevel(LogLevel.Trace)
            
            .ClearProviders()
            .AddConsoleFormatter<MyConsoleFormatter, MyConsoleFormatterOptions>()
            .AddConsole(config => config.FormatterName = nameof(MyConsoleFormatter))
            .SetMinimumLevel(LogLevel.Trace)
            ;
    })
    .Build();



var service = host.Services.GetService<EchoingService>();

service.WriteLocationToConsole();

public class LocationConfiguration
{
    public string Country { get; set; }
    public string City { get; set; }
}

public class LocationService
{
    private LocationConfiguration _configuration;
    private readonly ILogger<LocationService> _logger;

    public LocationService
    (
        IOptions<LocationConfiguration> configuration,
        ILogger<LocationService> logger
    )
    {
        _configuration = configuration.Value;
        _logger = logger;
    }

    public string GetAddress()
    {
        _logger.LogInformation("Requested location");
        return $"[{_configuration.Country}]{_configuration.City}";
    }
}

public class EchoingService
{
    private readonly LocationService _locationService;
    private readonly ILogger<EchoingService> _logger;

    public EchoingService
    (
        LocationService locationService,
        ILogger<EchoingService> logger
    )
    {
        _locationService = locationService;
        _logger = logger;
    }

    public void WriteLocationToConsole()
    {
        _logger.LogTrace("Hello world");
        _logger.LogDebug("Hello world");
        _logger.LogInformation("Hello world");
        _logger.LogWarning("Hello world");
        _logger.LogError("Hello world");
        _logger.LogCritical("Hello world");

        _logger.LogInformation("Echoing address to console...");
        _logger.LogInformation(_locationService.GetAddress());
        //Console.WriteLine(_locationService.GetAddress());
        _logger.LogInformation("Echoed address to console");
    }
}



