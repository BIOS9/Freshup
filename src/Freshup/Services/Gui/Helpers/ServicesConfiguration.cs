using Freshup.Services.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace Freshup.Services.Gui.Helpers;

public static class ServicesConfiguration
{
    public static void AddGui(this IServiceCollection services)
    {
        services.AddSingleton<MainForm>();
    }
}