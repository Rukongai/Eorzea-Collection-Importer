using System;
using System.Numerics;
using Dalamud.Interface.Windowing;
using ImGuiNET;

namespace ECImporter.Windows;

public class InstructionsWindow : Window, IDisposable
{
    private ECImporterPlugin ecimporter;
    public InstructionsWindow(ECImporterPlugin plugin) : base("Instructions")
    {
        Size = new Vector2(675, 250);
        ecimporter = plugin;
    }

    public void Dispose() { }

    public override void PreDraw() { }

    public override void Draw()
    {
        ImGui.Spacing();
        TextBullet("Copy a url from Eorzea Collection to your clipboard");
        TextBullet("Click the import button");
        TextBullet("This will copy a share string for glamourer to your clipboard");
        ImGui.Indent(16.0f);
        TextBullet("Look in Chatbox for [Eorzea Collection Importer] Import string copied to clipboard");
        ImGui.Unindent(16.0f);
        TextBullet("Open Glamourer and create a new design");
        TextBullet("While holding the CTRL key, click on the the Paste button on the design to import");
        ImGui.Separator();
        TextBullet("Currently only 1 of 2 rings will import");
        TextBullet("If you are not holding CTRL when importing, it will overwrite any character customizations you have on your Design");
        ImGui.Separator();
        if (ImGui.Button("Close"))
        {
            ecimporter.ToggleInstructionsUI();
        }
    }
    private static void TextBullet(string text)
    {
        ImGui.BulletText("");
        ImGui.SameLine();
        ImGui.TextWrapped(text);
    }
}
