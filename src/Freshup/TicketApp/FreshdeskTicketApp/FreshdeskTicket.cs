using DBA.FreshdeskSharp.Models;
using Freshup.TicketApp;

namespace Freshup.TicketApp.FreshdeskTicketApp;

public class FreshdeskTicket : ITicket
{
    public DBA.FreshdeskSharp.Models.FreshdeskTicket<FreshdeskCustomFields> Ticket { get; }
    public FreshdeskContact<FreshdeskCustomFields> Contact { get; private set; }
    public string? Subject => Ticket?.Subject;
    public string? Description => Ticket?.Description;
    public string? SenderEmail => Contact?.Email;
    public string? SenderName => Contact?.Name;
    public Uri? Link => new Uri($"https://{_freshdeskDomain}/a/tickets/{Ticket.Id}");

    private readonly string _freshdeskDomain;

    public FreshdeskTicket(DBA.FreshdeskSharp.Models.FreshdeskTicket<FreshdeskCustomFields> ticket, string freshdeskDomain)
    {
        _freshdeskDomain = freshdeskDomain ?? throw new ArgumentException(nameof(freshdeskDomain));
        Ticket = ticket ?? throw new ArgumentNullException(nameof(ticket));
    }

    public FreshdeskTicket WithContact(FreshdeskContact<FreshdeskCustomFields> contact)
    {
        FreshdeskTicket t = new FreshdeskTicket(Ticket, _freshdeskDomain);
        t.Contact = contact;
        return t;
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