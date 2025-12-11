using Dalamud.Game;
using Dalamud.Game.Command;
using Dalamud.Interface.Windowing;
using Dalamud.IoC;
using Dalamud.Plugin;
using Dalamud.Plugin.Services;
using System;
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
    [PluginService] internal static IGameInteropProvider GameInteropProvider {  get; private set; } = null!;
    [PluginService] internal static ISigScanner SigScanner { get; private set; } = null!;
    [PluginService] internal static IFramework Framework { get; private set; } = null!;
    [PluginService] internal static IObjectTable ObjectTable { get; private set; } = null!;
    [PluginService] internal static IChatGui ChatGui { get; private set; } = null!;

    //private const string Command5Name = "/ptest";
    private const string VfxPath = "vfx/common/eff/naelvoicelines.avfx";
    private const ushort UCoBId = 733;
    private const string LocalVfxFilename = "base_vfx.avfx";

    public Configuration Configuration { get; init; }

    public readonly WindowSystem WindowSystem = new("VoicedNaelLines");
    private ConfigWindow ConfigWindow { get; init; }
    private MainWindow MainWindow { get; init; }
    private GameFunctions gameFunctions { get; init; }
    private VfxSpawn vfxSpawn { get; init; }
    private ResourceLoader resourceLoader { get; init; }
    private QuoteHandler QuoteHandler { get; init; }

    public Plugin()
    {
        Configuration = PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();

        ConfigWindow = new ConfigWindow(this);
        MainWindow = new MainWindow(this);

        WindowSystem.AddWindow(ConfigWindow);
        WindowSystem.AddWindow(MainWindow);

        //CommandManager.AddHandler(Command5Name, new CommandInfo(OnTest)
        //{
        //    HelpMessage = "A useful message to display in /xlhelp"
        //});

        // Tell the UI system that we want our windows to be drawn throught he window system
        PluginInterface.UiBuilder.Draw += WindowSystem.Draw;

        // This adds a button to the plugin installer entry of this plugin which allows
        // toggling the display status of the configuration ui
        PluginInterface.UiBuilder.OpenConfigUi += ToggleConfigUi;

        // Adds another button doing the same but for the main ui of the plugin
        PluginInterface.UiBuilder.OpenMainUi += ToggleMainUi;


        gameFunctions = new GameFunctions();
        vfxSpawn = new VfxSpawn(gameFunctions);
        resourceLoader = new ResourceLoader(vfxSpawn);
        QuoteHandler = new QuoteHandler(this, resourceLoader, vfxSpawn);

        // TODO: need to handle case for plugin init in instance 

        ClientState.TerritoryChanged += OnTerritoryChanged;
    }

    public void Dispose()
    {
        ClientState.TerritoryChanged -= OnTerritoryChanged;
        vfxSpawn.Dispose();
        gameFunctions.Dispose();

        // Unregister all actions to not leak anythign during disposal of plugin
        PluginInterface.UiBuilder.Draw -= WindowSystem.Draw;
        PluginInterface.UiBuilder.OpenConfigUi -= ToggleConfigUi;
        PluginInterface.UiBuilder.OpenMainUi -= ToggleMainUi;
        
        WindowSystem.RemoveAllWindows();

        ConfigWindow.Dispose();
        MainWindow.Dispose();

        //CommandManager.RemoveHandler(Command5Name);
    }

    private void OnTerritoryChanged(ushort obj)
    {
        if (obj == UCoBId)
        {
            resourceLoader.AddFileReplacement(VfxPath, GetResourcePath(PluginInterface, LocalVfxFilename));
        } else
        {
            resourceLoader.RemoveFileReplacement(VfxPath);
        }
    }


    private void OnCommand(string command, string args)
    {
        // In response to the slash command, toggle the display status of our main ui
        MainWindow.Toggle();
    }

    //private void OnTest(string command, string args)
    //{
    //    var test = int.Parse(args);
    //    QuoteHandler.PlayNaelQuote((QuoteHandler.NaelQuote)test);
    //}

    public static string GetResourcePath(IDalamudPluginInterface pluginInterface, string fileName)
    {
        var resourcesDir = Path.Combine(pluginInterface.AssemblyLocation.Directory?.FullName!, "Resources");
        return Path.Combine(resourcesDir, fileName);
    }

    public void ToggleConfigUi() => ConfigWindow.Toggle();
    public void ToggleMainUi() => MainWindow.Toggle();
}
