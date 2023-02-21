using DBA.FreshdeskSharp;
using DBA.FreshdeskSharp.Models;
using System.Text.RegularExpressions;

namespace Freshup.TicketApp.FreshdeskTicketApp;

public class FreshdeskTicketApp : ITicketApp
{
    public static readonly TimeSpan MinimumTicketPollInterval = TimeSpan.FromSeconds(10);
    //public static readonly Regex DomainRegex = new Regex(@"^https:\/\/.+$", RegexOptions.IgnoreCase);
    public static readonly Regex DomainRegex = new Regex(@"^[a-z0-9\-]+\.freshdesk.com$", RegexOptions.IgnoreCase);
    public static readonly Regex ApiKeyRegex = new Regex(@"^[a-z0-9]{16,}$", RegexOptions.IgnoreCase);

    private readonly string _domain;
    private readonly string _apiKey;
    private readonly TimeSpan _pollingInterval;
    private readonly FreshdeskClient _freshdeskClient;
    private readonly CancellationTokenSource _pollingTokenSource = new();
    private HashSet<FreshdeskTicket> _tickets = new();

    private delegate void TicketsUpdatedEventHandler(object sender, IEnumerable<FreshdeskTicket> tickets);
    private event TicketsUpdatedEventHandler? TicketsUpdated;

    public event ITicketApp.NewTicketEventHandler? NewTicket;
    public event ITicketApp.ExceptionThrownEventHandler? ExceptionThrown;

    public FreshdeskTicketApp(string domain, string apiKey, TimeSpan pollingInterval)
    {
        _domain = domain ?? throw new ArgumentNullException(nameof(domain));
        _apiKey = apiKey ?? throw new ArgumentNullException(nameof(apiKey));
        _pollingInterval = pollingInterval;

        if (!DomainRegex.IsMatch(_domain))
            throw new ArgumentException("Freshdesk domain invalid.\n\n The domain should be in the format \"domain.freshdesk.com\"\nIt should not contain any slashes.");

        if (!ApiKeyRegex.IsMatch(_apiKey))
            throw new ArgumentException("Invalid Freshdesk API key");

        if (_pollingInterval < MinimumTicketPollInterval)
            throw new ArgumentException($"TicketPollInterval is too small. Minimum is: {MinimumTicketPollInterval.TotalSeconds}s");

        var credentials = new FreshdeskCredentials(apiKey);
        var config = new FreshdeskConfig
        {
            Domain = domain,
            Credentials = credentials,
            RetryWhenThrottled = false
        };
        _freshdeskClient = new FreshdeskClient(config);
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
        bool firstRun = true;
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                /* A note on this delay:
                 * This technically means the loop does not run using the interval specified in the settings, but with interval + download time
                 * since it waits for the download and then waits for another interval.
                 *
                 * This can be changed later if needed.
                 */
                await Task.Delay(_pollingInterval, cancellationToken);

                var existingTickets = _tickets; // Defensive reference
                var newTickets = new HashSet<FreshdeskTicket>();

                var listOptions = new FreshdeskTicketListOptions()
                {
                    Filter = FreshdeskTicketFilter.NewAndMyOpen,
                    OrderType = FreshdeskOrderType.Desc,
                    OrderBy = FreshdeskTicketOrderBy.CreatedAt,
                    Page = 1,
                    PerPage = 10
                };

                var tickets = await _freshdeskClient.Tickets.GetListAsync(listOptions);
                foreach (var ticket in tickets)
                {
                    var hashableTicket = new TicketApp.FreshdeskTicketApp.FreshdeskTicket(ticket, _domain);

                    if (!firstRun && !existingTickets.Contains(hashableTicket))
                    {
                        //try
                        //{
                        //    //var newTicket = new FreshdeskTicket(await _freshdeskClient.Tickets.ViewTicketAsync(hashableTicket.Ticket.Id));
                        //    var contact = await _freshdeskClient.Contacts.ViewContactAsync(ticket.RequesterId);
                        //    ticket.Requester = new Requester();
                        //    ticket.Requester.Name = contact.Name;
                        //    ticket.Requester.Email = contact.Email;
                        //}
                        //catch(Exception ex)
                        //{
                        //    // Ignore for now, need to fix
                        //}
                        NewTicket?.Invoke(this, hashableTicket);
                    }

                    newTickets.Add(hashableTicket);
                }

#if DEBUG
                // show a test ticket in debug mode
                if (firstRun)
                {
                    NewTicket?.Invoke(this, new TestTicket());
                }
#endif

                TicketsUpdated?.Invoke(this, newTickets);
                _tickets = newTickets;
                firstRun = false;
            }
            catch (TaskCanceledException ex) { }
            catch (Exception ex)
            {
                ExceptionThrown?.Invoke(this, ex);
                await Task.Delay(TimeSpan.FromMinutes(10), cancellationToken);
            }
        }
    }

    private class TestTicket : ITicket
    {
        public string? Subject => "Subject";
        public string? Description => "Description";
        public string? SenderEmail => "Email";
        public string? SenderName => "Sender name";
        public Uri? Link => new Uri("https://google.com");
    }

    public void Dispose()
    {
        _freshdeskClient.Dispose();
    }
}