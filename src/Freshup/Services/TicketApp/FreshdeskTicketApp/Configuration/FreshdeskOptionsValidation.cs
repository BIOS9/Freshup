using Microsoft.Extensions.Options;

namespace Freshup.Services.FreshdeskTicketApp.Configuration;

public class FreshdeskOptionsValidation : IValidateOptions<FreshdeskOptions>
{
    public ValidateOptionsResult Validate(string name, FreshdeskOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.ApiKey))
        {
            return ValidateOptionsResult.Fail("Missing API key.");
        }

        return ValidateOptionsResult.Success;
    }
}