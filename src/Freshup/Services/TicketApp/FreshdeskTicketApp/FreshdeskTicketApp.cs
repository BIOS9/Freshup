using Microsoft.Extensions.Logging;

namespace Freshup.Services.FreshdeskTicketApp;

public class FreshdeskTicketApp : ITicketApp
{
    private readonly ILogger<FreshdeskTicketApp> _logger;

    public FreshdeskTicketApp(ILogger<FreshdeskTicketApp> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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