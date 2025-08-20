using DiscordLab.Bot.API.Attributes;
using DiscordLab.Bot.API.Features;
using DiscordLab.Dependency;
using HarmonyLib;
using LabApi.Events.CustomHandlers;
using LabApi.Features;

namespace DiscordLab.Administration;

public class Plugin : Plugin<Config, Translation>
{
    public static Plugin Instance;
        
    public override string Name { get; } = "DiscordLab.Administration";

    public override string Description { get; } =
        "Allows you to log or do administrative actions from your Discord bot";

    public override string Author { get; } = "LumiFae";
    public override Version Version => GetType().Assembly.GetName().Version;
    public override Version RequiredApiVersion { get; } = new(LabApiProperties.CompiledVersion);

    public Events Events = new();

    private Harmony harmony = new($"DiscordLab.Administration-{DateTime.Now.Ticks}");
        
    public override void Enable()
    {
        Instance = this;
        harmony.PatchAll();
        CallOnLoadAttribute.Load();
            
        if(Config.AddCommands)
            SlashCommand.FindAll();
            
        CustomHandlersManager.RegisterEventsHandler(Events);
    }

    public override void Disable()
    {
        CustomHandlersManager.UnregisterEventsHandler(Events);
        CallOnUnloadAttribute.Unload();
        harmony.UnpatchAll();
        Events = null;
        Instance = null;
    }
}