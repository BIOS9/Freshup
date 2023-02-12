using FreshdeskApi.Client.Tickets.Models;
using Freshup.Services.TicketApp;

namespace Freshup.Services.TicketApp.FreshdeskTicketApp;

public class FreshdeskTicket : ITicket
{
    public Ticket Ticket { get; }
    public string? Subject => Ticket?.Subject;
    public string? Description => Ticket?.Description;
    public string? SenderEmail => Ticket?.Requester?.Email;
    public string? SenderName => Ticket?.Requester?.Name;
    public Uri? Link => null;


    public FreshdeskTicket(Ticket ticket)
    {
        Ticket = ticket ?? throw new ArgumentNullException(nameof(ticket));
    }

    public override bool Equals(object? obj)
    {
        if (obj == null) return false;
        if (obj.GetType() != GetType()) return false;
        return ((FreshdeskTicket)obj).Ticket.Id == Ticket.Id;
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