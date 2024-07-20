using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;

namespace ECImporter;

internal class Service
{
    [PluginService] internal static ICommandManager CommandManager { get; private set; }
    [PluginService] internal static IDataManager DataManager { get; private set; }
    [PluginService] internal static IPluginLog PluginLog { get; private set; }
    [PluginService] internal static IAddonLifecycle AddonLifecycle { get; private set; }
    [PluginService] public static IChatGui Chat { get; private set; }

    internal static void Initialize(IDalamudPluginInterface pluginInterface)
    {
        pluginInterface.Create<Service>();
    }
}
