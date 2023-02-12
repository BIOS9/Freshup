using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Squirrel.Sources;
using Squirrel;

namespace Freshup;

public class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        CheckUpdates();
        ApplicationConfiguration.Initialize();
        IHost host = CreateHostBuilder(args).Build();
        host.Services.GetRequiredService<Startup>().Run();
    }

    private static void CheckUpdates()
    {
        SquirrelAwareApp.HandleEvents(
            onInitialInstall: OnAppInstall,
            onAppUninstall: OnAppUninstall,
            onEveryRun: OnAppRun);

        using (var mgr = new UpdateManager(new GithubSource("https://github.com/BIOS9/Freshup", string.Empty, false)))
        {
            if (mgr.IsInstalledApp)
            {
                var newVersion = mgr.UpdateApp().Result;

                if (newVersion != null)
                {
                    UpdateManager.RestartApp();
                }
            }
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
    {
        // Create generic host and use startup service.
        // We use a startup service here instead of just adding each individual service
        // directly since some services require setup with things like configuration options.
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
                services.AddSingleton<Startup>())
            .ConfigureAppConfiguration((context, builder) =>
                builder.AddUserSecrets<Program>());
    }

    private static void OnAppInstall(SemanticVersion version, IAppTools tools)
    {
        tools.CreateShortcutForThisExe(ShortcutLocation.StartMenu);
    }

    private static void OnAppUninstall(SemanticVersion version, IAppTools tools)
    {
        tools.RemoveShortcutForThisExe(ShortcutLocation.StartMenu);
    }

    private static void OnAppRun(SemanticVersion version, IAppTools tools, bool firstRun)
    {
        tools.SetProcessAppUserModelId();
    }
}