using Freshup.Services;
using Freshup.Services.FreshdeskTicketApp.Helpers;
using Freshup.Services.Testing;
using Freshup.Services.Testing.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace Freshup;

public class Startup : IHostedService
{
    private readonly IConfiguration _configuration;
    private IEnumerable<IHostedService> _runningServices = Enumerable.Empty<IHostedService>();
        
    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(_configuration)
            .CreateLogger();
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Log.Information("Application starting...");
        // Set up services for dependency injection.
        var services = new ServiceCollection();
        ConfigureServices(services);
        var provider = services.BuildServiceProvider();

        _runningServices = new IHostedService[]
        {
            provider.GetRequiredService<ITicketApp>(),
            provider.GetRequiredService<Testing>()
        };
        await Task.WhenAll(_runningServices.Select(s => 
            s.StartAsync(cancellationToken)));
        Log.Information("Application started");
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        Log.Information("Application stopping...");
        await Task.WhenAll(_runningServices.Select(s => 
            s.StopAsync(cancellationToken)));
        Log.Information("Application stopped");
    }
        
    /// <summary>
    /// Configures services to be dependency injected.
    /// </summary>
    private void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(x => x.AddSerilog());
        services.AddFreshdesk(_configuration);
        services.AddTesting();
    }
}