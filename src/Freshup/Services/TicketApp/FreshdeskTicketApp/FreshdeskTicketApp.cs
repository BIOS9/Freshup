using FreshdeskApi.Client;
using FreshdeskApi.Client.Tickets.Requests;
using Freshup.Services.FreshdeskTicketApp.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TimeSpanParserUtil;

namespace Freshup.Services.FreshdeskTicketApp;

public class FreshdeskTicketApp : ITicketApp
{
    private readonly FreshdeskOptions _options;
    private readonly ILogger<FreshdeskTicketApp> _logger;
    private readonly FreshdeskClient _freshdeskClient;
    private readonly CancellationTokenSource _pollingTokenSource = new CancellationTokenSource();
    private HashSet<HashableTicket> _tickets = new HashSet<HashableTicket>();

    public FreshdeskTicketApp(IOptions<FreshdeskOptions> options, ILogger<FreshdeskTicketApp> logger)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        
        var freshdeskHttpClient = FreshdeskHttpClient.Create(_options.Domain, _options.ApiKey);
        _freshdeskClient = FreshdeskClient.Create(freshdeskHttpClient);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        StartPolling(_pollingTokenSource.Token);
        _logger.LogInformation("Freshdesk monitoring started");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _pollingTokenSource.Cancel();
        return Task.CompletedTask;
    }

    public Task<ITicket> GetTicketsAsync()
    {
        throw new NotImplementedException();
    }

    private async void StartPolling(CancellationToken cancellationToken)
    {
        #if !DEBUG
        await Task.Delay(TimeSpan.FromSeconds(30));
        #endif

        bool firstRun = true;
        
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var existingTickets = _tickets; // Defensive reference
                var newTickets = new HashSet<HashableTicket>();
                
                await foreach (var ticket in _freshdeskClient.Tickets.ListAllTicketsAsync(
                                   new ListAllTicketsRequest(),
                                   new PaginationConfiguration(),
                                   cancellationToken))
                {
                    var hashTicket = new HashableTicket(ticket);

                    if (!firstRun && !existingTickets.Contains(hashTicket))
                    {
                        _logger.LogInformation("NEW TICKET! {Subject}", ticket.Subject);
                    }

                    newTickets.Add(hashTicket);
                }

                _tickets = newTickets;
                firstRun = false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occured in ticket polling loop {Message}", ex.Message);
                await Task.Delay(TimeSpan.FromMinutes(5), cancellationToken);
            }
            await Task.Delay(TimeSpanParser.Parse(_options.TicketPollInterval));
        }
    }
}