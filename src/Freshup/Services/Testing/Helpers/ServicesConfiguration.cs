using Microsoft.Extensions.DependencyInjection;

namespace Freshup.Services.Testing.Helpers;

public static class ServicesConfiguration
{
    public static void AddTesting(this IServiceCollection services)
    {
        services.AddSingleton<Testing>();
    }
}