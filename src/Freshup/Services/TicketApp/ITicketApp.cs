using Freshup.Services.FreshdeskTicketApp;
using Freshup.Services.TicketApp;
using Microsoft.Extensions.Hosting;

namespace Freshup.Services;

public interface ITicketApp
{
    void Start();
    void Stop();
    Task<IEnumerable<ITicket>> GetTicketsAsync();

    delegate void NewTicketEventHandler(object sender, ITicket ticket);
    event NewTicketEventHandler NewTicket;
}