using Freshup.Services.TicketApp.FreshdeskTicketApp.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Freshup.Services.TicketApp.FreshdeskTicketApp.Helpers;

public static class ServicesConfiguration
{
    public static void AddFreshdesk(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<FreshdeskOptions>(configuration.GetSection(FreshdeskOptions.Name));
        services.AddSingleton<IValidateOptions<FreshdeskOptions>, FreshdeskOptionsValidation>();
        services.AddScoped<ITicketApp, FreshdeskTicketApp>();
    }
}