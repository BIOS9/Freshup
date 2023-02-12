using Freshup.Services;
using Freshup.Services.FreshdeskTicketApp.Helpers;
using Freshup.Services.Gui;
using Freshup.Services.Gui.Helpers;
using Freshup.Services.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Squirrel;
using Squirrel.Sources;

namespace Freshup;

public class Startup
{
    private readonly IConfiguration _configuration;
        
    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(_configuration)
            .CreateLogger();
    }

    public void Run()
    {
        Log.Information("Update test 2.");
        Log.Information("Application starting...");
        // Set up services for dependency injection.
        var services = new ServiceCollection();
        ConfigureServices(services);
        var provider = services.BuildServiceProvider();

        provider.GetRequiredService<ITicketApp>().Start();
        provider.GetRequiredService<Gui>().Run();
        Log.Information("Application started");
    }

    /// <summary>
    /// Configures services to be dependency injected.
    /// </summary>
    private void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(x => x.AddSerilog());
        services.AddFreshdesk(_configuration);
        services.AddGui();
    }

    #region UPDATES

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

    #endregion
}