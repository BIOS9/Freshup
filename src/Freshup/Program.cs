using Squirrel.Sources;
using Squirrel;
using Freshup.Services.Testing;

namespace Freshup;

public class Program
{
    public static SemanticVersion CurrentVersion { get; private set; }

    [STAThread]
    static async Task Main(string[] args)
    {
        SquirrelAwareApp.HandleEvents(
            onInitialInstall: OnAppInstall,
            onAppUninstall: OnAppUninstall,
            onEveryRun: OnAppRun);

        await CheckUpdates();
        DoUpdateLoop();
        ApplicationConfiguration.Initialize();
        Application.Run(new SettingsForm());
    }

    #region UPDATES
    
    private static async void DoUpdateLoop()
    {
        while(true)
        {
            try
            {
                await Task.Delay(TimeSpan.FromHours(1));
                await CheckUpdates();
            }
            catch
            {
                await Task.Delay(TimeSpan.FromHours(2));
            }
        }
    }

    private static async Task CheckUpdates()
    {
        using (var mgr = new UpdateManager(new GithubSource("https://github.com/BIOS9/Freshup", string.Empty, false)))
        {
            if (mgr.IsInstalledApp)
            {
                var newVersion = await mgr.UpdateApp();

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
        CurrentVersion = version;
        tools.SetProcessAppUserModelId();
    }

    #endregion
}