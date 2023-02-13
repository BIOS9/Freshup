namespace Freshup.Services.TicketApp;

public interface ITicketApp
{
    void Start();
    void Stop();
    Task<IEnumerable<ITicket>> GetTicketsAsync();

    delegate void NewTicketEventHandler(object sender, ITicket ticket);
    event NewTicketEventHandler NewTicket;
}