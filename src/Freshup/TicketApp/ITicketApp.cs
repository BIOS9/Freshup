namespace Freshup.Services.TicketApp;

public interface ITicketApp : IDisposable
{
    void Start();
    void Stop();
    Task<IEnumerable<ITicket>> GetTicketsAsync();

    delegate void NewTicketEventHandler(object sender, ITicket ticket);
    event NewTicketEventHandler NewTicket;

    delegate void ExceptionThrownEventHandler(object sender, Exception ex);
    event ExceptionThrownEventHandler ExceptionThrown;
}