namespace Freshup.Services.TicketApp;

public interface ITicket
{
    string? Subject { get; }
    string? Description { get; }
    string? SenderEmail { get; }
    string? SenderName { get; }
    Uri? Link { get; }
}