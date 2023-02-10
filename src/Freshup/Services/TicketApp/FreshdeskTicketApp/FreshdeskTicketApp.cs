﻿using FreshdeskApi.Client;
using FreshdeskApi.Client.Tickets.Requests;
using Freshup.Services.FreshdeskTicketApp.Configuration;
using Freshup.Services.TicketApp;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TimeSpanParserUtil;

namespace Freshup.Services.FreshdeskTicketApp;

public class FreshdeskTicketApp : ITicketApp
{
    private readonly FreshdeskOptions _options;
    private readonly ILogger<FreshdeskTicketApp> _logger;
    private readonly FreshdeskClient _freshdeskClient;
    private readonly CancellationTokenSource _pollingTokenSource = new ();
    private HashSet<FreshdeskTicket> _tickets = new ();
    
    public delegate void TicketsUpdatedEventHandler(object sender, IEnumerable<FreshdeskTicket> tickets);
    public event TicketsUpdatedEventHandler TicketsUpdated;
    
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

    public async Task<IEnumerable<ITicket>> GetTicketsAsync()
    {
        TaskCompletionSource<IEnumerable<FreshdeskTicket>> tcs = new ();
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
                        _logger.LogInformation("NEW TICKET! {Subject}", ticket.Subject);
                    }

                    newTickets.Add(hashTicket);
                }

                TicketsUpdated?.Invoke(this, newTickets);
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