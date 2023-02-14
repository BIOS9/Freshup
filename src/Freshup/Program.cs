using Squirrel.Sources;
using Squirrel;
using Freshup.Services.Gui;

namespace Freshup;

public class Program
{
    [STAThread]
    static void Main(string[] args)
    {
        CheckUpdates();
        ApplicationConfiguration.Initialize();
        Application.Run(new Gui());
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