using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Freshup.Services.Testing;

public class Testing : IHostedService
{
    private readonly ILogger<Testing> _logger;
    private readonly ITicketApp _ticketApp;

    public Testing(ILogger<Testing> logger, ITicketApp ticketApp)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _ticketApp = ticketApp ?? throw new ArgumentNullException(nameof(ticketApp));
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Run();
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private async void Run()
    {
        try
        {
            _logger.LogInformation("Run started");
            for (int i = 0; i < 5; ++i)
            {
                var tickets = await _ticketApp.GetTicketsAsync();
                _logger.LogInformation("Got tickets");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error in testing run");
        }
    }
}