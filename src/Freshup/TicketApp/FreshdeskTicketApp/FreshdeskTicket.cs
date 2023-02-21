using DBA.FreshdeskSharp.Models;
using Freshup.TicketApp;

namespace Freshup.TicketApp.FreshdeskTicketApp;

public class FreshdeskTicket : ITicket
{
    public DBA.FreshdeskSharp.Models.FreshdeskTicket<FreshdeskCustomFields> Ticket { get; }
    public string? Subject => Ticket?.Subject;
    public string? Description => Ticket?.Description;
    public string? SenderEmail => Ticket?.Email;
    public string? SenderName => Ticket?.Name;
    public Uri? Link => new Uri($"{_freshdeskDomain}/a/tickets/{Ticket.Id}");

    private readonly string _freshdeskDomain;

    public FreshdeskTicket(DBA.FreshdeskSharp.Models.FreshdeskTicket<FreshdeskCustomFields> ticket, string freshdeskDomain)
    {
        _freshdeskDomain = freshdeskDomain ?? throw new ArgumentException(nameof(freshdeskDomain));
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