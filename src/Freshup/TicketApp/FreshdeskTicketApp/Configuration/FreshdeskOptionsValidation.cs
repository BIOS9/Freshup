using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using TimeSpanParserUtil;

namespace Freshup.Services.TicketApp.FreshdeskTicketApp.Configuration;

public class FreshdeskOptionsValidation : IValidateOptions<FreshdeskOptions>
{
    private static readonly TimeSpan MinimumTicketPollInterval = TimeSpan.FromMilliseconds(1500);
    
    public ValidateOptionsResult Validate(string? name, FreshdeskOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.Domain))
        {
            return ValidateOptionsResult.Fail("Missing Freshdesk domain");
        }
        
        if (Regex.IsMatch(options.Domain, @"/^https:\/\/.+"))
        {
            return ValidateOptionsResult.Fail("Freshdesk domain invalid. Ensure it starts with https://");
        }
        
        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            return ValidateOptionsResult.Fail("Missing API key");
        }

        if (!TimeSpanParser.TryParse(options.TicketPollInterval, out TimeSpan ticketPollInterval))
        {
            return ValidateOptionsResult.Fail("Failed to parse TicketPollInterval. Use format \"1m10s\"");
        }
        
        if (ticketPollInterval < MinimumTicketPollInterval)
        {
            return ValidateOptionsResult.Fail($"TicketPollInterval is too small. Minimum is: {MinimumTicketPollInterval.TotalSeconds}s");
        }

        return ValidateOptionsResult.Success;
    }
}