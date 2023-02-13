using Freshup.Services.Gui;
using Freshup.Services.Gui.Helpers;
using Freshup.Services.TicketApp;
using Freshup.Services.TicketApp.FreshdeskTicketApp.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
        Log.Information("Checking for updates...");
        CheckUpdates();
        Log.Information("Application starting...");

        Log.Information("Application started");
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