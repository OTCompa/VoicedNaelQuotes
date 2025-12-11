using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using System.IO;
using VoicedNaelLines.Interop;
using VoicedNaelLines.Windows;

namespace VoicedNaelLines;

public sealed class Plugin : IDalamudPlugin
{
    [PluginService] internal static IDalamudPluginInterface PluginInterface { get; private set; } = null!;
    [PluginService] internal static ITextureProvider TextureProvider { get; private set; } = null!;
    [PluginService] internal static ICommandManager CommandManager { get; private set; } = null!;
    [PluginService] internal static IClientState ClientState { get; private set; } = null!;
    [PluginService] internal static IDataManager DataManager { get; private set; } = null!;
    [PluginService] internal static IPluginLog Log { get; private set; } = null!;
    [PluginService] internal static IGameInteropProvider gameInteropProvider {  get; private set; } = null!;
    [PluginService] internal static ISigScanner sigScanner { get; private set; } = null!;
    [PluginService] internal static IFramework framework { get; private set; } = null!;
    [PluginService] internal static IObjectTable objectTable { get; private set; } = null!;

    private const string CommandName = "/pspawnvfx";
    private const string Command2Name = "/pclearvfx";
    private const string Command3Name = "/preplacevfx";
    private const string Command4Name = "/premovevfx";
    private const string VfxPath = "vfx/common/eff/naelvoicelines.avfx";
    private const string ScdPath = "sound/vfx/monster8/naelvoicelines.scd";
    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("VoicedNaelLines");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }
    private GameFunctions gameFunctions { get; init; }
    private VfxSpawn vfxSpawn { get; init; }
    private ResourceLoader resourceLoader { get; init; }

    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        CommandManager.AddHandler(CommandName, new CommandInfo(OnSpawnCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });

        CommandManager.AddHandler(Command2Name, new CommandInfo(OnRemoveCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });

        CommandManager.AddHandler(Command3Name, new CommandInfo(OnReplaceCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });

        CommandManager.AddHandler(Command4Name, new CommandInfo(OnRemoveReplacementCommand)
        {
            HelpMessage = "A useful message to display in /xlhelp"
        });

        // Tell the UI system that we want our windows to be drawn throught he window system
        PluginInterface.UiBuilder.Draw += WindowSystem.Draw;

        // This adds a button to the plugin installer entry of this plugin which allows
        // toggling the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUi;

        // Adds another button doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUi;

        // Add a simple message to the log with level set to information
        // Use /xllog to open the log window in-game
        // Example Output: 00:57:54.959 | INF | [SamplePlugin] ===A cool log message from Sample Plugin===
        Log.Information($"===A cool log message from {PluginInterface.Manifest.Name}===");

        gameFunctions = new GameFunctions(sigScanner, gameInteropProvider);
        vfxSpawn = new VfxSpawn(gameFunctions, framework);
        resourceLoader = new ResourceLoader(sigScanner, gameInteropProvider, vfxSpawn);
    }

    public void Dispose()
    {
        vfxSpawn.Dispose();
        gameFunctions.Dispose();

        // Unregister all actions to not leak anythign during disposal of plugin
        PluginInterface.UiBuilder.Draw -= WindowSystem.Draw;
        PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigUi;
        PluginInterface.UiBuilder.OpenMainUi -= ToggleMainUi;
        
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        CommandManager.RemoveHandler(Command4Name);
        CommandManager.RemoveHandler(Command3Name);
        CommandManager.RemoveHandler(Command2Name);
        CommandManager.RemoveHandler(CommandName);
    }

    private void OnCommand(string command, string args)
    {
        // In response to the slash command, toggle the display status of our main ui
        MainWindow.Toggle();
    }
    
    private void OnSpawnCommand(string command, string args)
    {
        if (!resourceLoader.GetReplacePath(VfxPath, out _) || !resourceLoader.GetReplacePath(ScdPath, out _)) return;
        if (objectTable.LocalPlayer == null) return;

        if (objectTable.LocalPlayer.TargetObject != null)
        {
            vfxSpawn.QueueActorVfx(VfxPath, objectTable.LocalPlayer.TargetObject);
        } else
        {
            vfxSpawn.QueueActorVfx(VfxPath, objectTable.LocalPlayer);
        }
    }

    private void OnRemoveCommand(string command, string args)
    {
        vfxSpawn.DespawnAllVfxes();
    }

    private void OnReplaceCommand(string command, string args)
    {
        resourceLoader.AddFileReplacement(ScdPath, GetResourcePath(PluginInterface, "stack_in.scd"));
        resourceLoader.AddFileReplacement(VfxPath, GetResourcePath(PluginInterface, "base_vfx.avfx"));
    }

    private void OnRemoveReplacementCommand(string command, string args)
    {
        resourceLoader.RemoveFileReplacement(VfxPath);
        resourceLoader.RemoveFileReplacement(ScdPath);
    }
    public static string GetResourcePath(IDalamudPluginInterface pluginInterface, string fileName)
    {
        var resourcesDir = Path.Combine(pluginInterface.AssemblyLocation.Directory?.FullName!, "Resources");
        return Path.Combine(resourcesDir, fileName);
    }

    public void ToggleConfigUi() => ConfigWindow.Toggle();
    public void ToggleMainUi() => MainWindow.Toggle();
}
