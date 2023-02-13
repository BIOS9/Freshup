using FreshdeskApi.Client;
using FreshdeskApi.Client.Tickets.Requests;
using Freshup.Services.TicketApp.FreshdeskTicketApp.Configuration;
using Microsoft.Extensions.Options;
using TimeSpanParserUtil;

namespace Freshup.Services.TicketApp.FreshdeskTicketApp;

public class FreshdeskTicketApp : ITicketApp
{
    private readonly FreshdeskOptions _options;
    private readonly FreshdeskClient _freshdeskClient;
    private readonly CancellationTokenSource _pollingTokenSource = new();
    private HashSet<FreshdeskTicket> _tickets = new();

    private delegate void TicketsUpdatedEventHandler(object sender, IEnumerable<FreshdeskTicket> tickets);
    private event TicketsUpdatedEventHandler? TicketsUpdated;

    public event ITicketApp.NewTicketEventHandler? NewTicket;
    public event ITicketApp.ExceptionThrownEventHandler? ExceptionThrown;

    public FreshdeskTicketApp(IOptions<FreshdeskOptions> options)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));

        var freshdeskHttpClient = FreshdeskHttpClient.Create(_options.Domain, _options.ApiKey);
        _freshdeskClient = FreshdeskClient.Create(freshdeskHttpClient);
    }

    public void Start()
    {
        StartPolling(_pollingTokenSource.Token);
    }

    public void Stop()
    {
        using (_pollingTokenSource)
        {
            _pollingTokenSource.Cancel();
        }
    }

    public async Task<IEnumerable<ITicket>> GetTicketsAsync()
    {
        TaskCompletionSource<IEnumerable<FreshdeskTicket>> tcs = new();
        void Handler(object s, IEnumerable<FreshdeskTicket> t) => tcs.SetResult(t);
        TicketsUpdated += Handler;
        var tickets = await tcs.Task;
        TicketsUpdated -= Handler;
        return tickets;
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
                var newTickets = new HashSet<FreshdeskTicket>();

                await foreach (var ticket in _freshdeskClient.Tickets.ListAllTicketsAsync(
                                   new ListAllTicketsRequest(),
                                   new PaginationConfiguration(),
                                   cancellationToken))
                {
                    var hashTicket = new FreshdeskTicket(ticket);

                    if (!firstRun && !existingTickets.Contains(hashTicket))
                    {
                        NewTicket?.Invoke(this, hashTicket);
                    }

                    newTickets.Add(hashTicket);
                }

                TicketsUpdated?.Invoke(this, newTickets);
                _tickets = newTickets;
                firstRun = false;
            }
            catch (Exception ex)
            {
                ExceptionThrown?.Invoke(this, ex);
                await Task.Delay(TimeSpan.FromMinutes(10), cancellationToken);
            }
            /* A note on this delay:
             * This technically means the loop does not run using the interval specified in the settings, but with interval + download time
             * since it waits for the download and then waits for another interval.
             *
             * This can be changed later if needed.
             */
            await Task.Delay(TimeSpanParser.Parse(_options.TicketPollInterval), cancellationToken);
        }
    }
}