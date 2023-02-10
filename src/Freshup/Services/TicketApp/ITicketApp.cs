using Freshup.Services.TicketApp;
using Microsoft.Extensions.Hosting;

namespace Freshup.Services;

public interface ITicketApp : IHostedService
{
    Task<IEnumerable<ITicket>> GetTicketsAsync();
}