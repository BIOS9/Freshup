using FreshdeskApi.Client.Tickets.Models;

namespace Freshup.Services.FreshdeskTicketApp;

public class HashableTicket
{
    public Ticket Ticket { get; }

    public HashableTicket(Ticket ticket)
    {
        Ticket = ticket ?? throw new ArgumentNullException(nameof(ticket));
    }

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        if (obj.GetType() != GetType()) return false;
        return ((HashableTicket)obj).Ticket.Id == Ticket.Id;
    }

    public override int GetHashCode()
    {
        return Ticket?.Id.GetHashCode() ?? 0;
    }

    public override string ToString()
    {
        return Ticket.ToString();
    }
}