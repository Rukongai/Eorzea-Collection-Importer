using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using Dalamud.Plugin.Services;
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
            MinimumSize = new Vector2(200, 75),
            MaximumSize = new Vector2(float.MaxValue, float.MaxValue)
        };
        ecimporter = plugin;
    }

    public void Dispose() { }

    public override void Draw()
    {
        ImGui.Spacing();
        if (ImGui.Button("Import"))
        {
            string ecurl = ImGui.GetClipboardText();
            _ = Scrape.GetEC(ecurl);
        }
        ImGui.SameLine();
        if (ImGui.Button("Instructions"))
        {
            ecimporter.ToggleInstructionsUI();
        }
    }
}
