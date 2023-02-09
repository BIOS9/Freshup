namespace Freshup.Services.FreshdeskTicketApp.Configuration;

public class FreshdeskOptions
{
    public const string Name = "Freshdesk";

    public string Domain { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}