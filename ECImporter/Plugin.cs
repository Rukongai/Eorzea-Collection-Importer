using Dalamud.Game.Command;
using Dalamud.Plugin;
using Dalamud.Interface.Windowing;
using ECImporter.Windows;

namespace ECImporter;

public class ECImporterPlugin : IDalamudPlugin
{
    private MainWindow MainWindow { get; init; }
    private InstructionsWindow InstructionsWindow { get; init; }
    public readonly WindowSystem WindowSystem = new("ECImporter");
    private const string CommandName = "/ecimporter";
    public ECImporterPlugin(IDalamudPluginInterface pluginInterface)
    {
        Service.Initialize(pluginInterface);
        MainWindow = new MainWindow(this);
        InstructionsWindow = new InstructionsWindow(this);
        WindowSystem.AddWindow(MainWindow);
        WindowSystem.AddWindow(InstructionsWindow);

        Service.CommandManager.AddHandler(CommandName, new CommandInfo(OnCommand)
        {
            HelpMessage = "Import looks from Eorzea Collection"
        });
        pluginInterface.UiBuilder.Draw += DrawUI;
        pluginInterface.UiBuilder.OpenMainUi += ToggleMainUI;
    }

    public void Dispose()
    {
        WindowSystem.RemoveAllWindows();
        MainWindow.Dispose();
        InstructionsWindow.Dispose();
        Service.CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        ToggleMainUI();
    }

    private void DrawUI() => WindowSystem.Draw();
    public void ToggleMainUI() => MainWindow.Toggle();
    public void ToggleInstructionsUI() => InstructionsWindow.Toggle();
}
