using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(configBuilder =>
    {
        var environmentName = Environment.GetEnvironmentVariable("Environment");

        configBuilder.SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environmentName}.json", optional: true, reloadOnChange: true)
            .AddCommandLine(args)
            .AddEnvironmentVariables();
    })
    .ConfigureServices((hostContext, services) =>
    {
        services.Configure<LocationConfiguration>(hostContext.Configuration.GetSection("Location"));
        services.AddTransient<LocationService>();
        services.AddTransient<EchoingService>();
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
        _logger.LogInformation("Echoing address to console...");
        Console.WriteLine(_locationService.GetAddress());
        _logger.LogInformation("Echoed address to console");
    }
}