using FreshdeskApi.Client;
using Freshup.Services.FreshdeskTicketApp.Configuration;
using Microsoft.Extensions.Logging;

namespace Freshup.Services.FreshdeskTicketApp;

public class FreshdeskTicketApp : ITicketApp
{
    private readonly FreshdeskOptions _options;
    private readonly ILogger<FreshdeskTicketApp> _logger;
    private readonly FreshdeskClient _freshdeskClient;
    
    public FreshdeskTicketApp(FreshdeskOptions options, ILogger<FreshdeskTicketApp> logger)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        using var freshdeskHttpClient = FreshdeskHttpClient.Create(options.Domain, options.ApiKey);
        _freshdeskClient = FreshdeskClient.Create(freshdeskHttpClient);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Freshdesk monitoring started");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}