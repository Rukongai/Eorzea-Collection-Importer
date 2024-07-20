using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace ECImporter.Windows;

public class MainWindow : Window, IDisposable
{
    private ECImporterPlugin ecimporter;

    public MainWindow(ECImporterPlugin plugin)
        : base("Eorzea Collection Importer", ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse)
    {
        SizeConstraints = new WindowSizeConstraints
        {
            MinimumSize = new Vector2(375, 275),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
        ecimporter = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.Spacing();
        ImGui.Text("⬤ Copy a url from Eorzea Collection to your clipboard");
        ImGui.Text("⬤ Click the import button");
        ImGui.Text("⬤ This will copy a share string for glamourer to your clipboard");
        ImGui.Text("⬤ Open Glamourer and create a new design");
        ImGui.Text("⬤ While holding the ctrl key, click on the the paste button in glamourer");
        ImGui.Spacing();
        ImGui.Text("⬤ Currently only 1 ring will import and this runs like shit.");
        if (ImGui.Button("Import"))
        {
            string ecurl = ImGui.GetClipboardText();
            _ = Scrape.GetEC(ecurl);
        }

    }
}
