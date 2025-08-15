using DiscordLab.Bot.API.Attributes;
using DiscordLab.Bot.API.Features;
using DiscordLab.Dependency;
using LabApi.Events.CustomHandlers;
using LabApi.Features;

namespace DiscordLab.RoundLogs;

public class Plugin : Plugin<Config, Translation>
{
    public static Plugin Instance;
        
    public override string Name { get; } = "DiscordLab.RoundLogs";
    public override string Description { get; } = "Allows you to log specific details about the round.";
    public override string Author { get; } = "LumiFae";
    public override Version Version { get; } = typeof(Plugin).Assembly.GetName().Version;
    public override Version RequiredApiVersion { get; } = new(LabApiProperties.CompiledVersion);

    public Events Events = new();
        
    public override void Enable()
    {
        Instance = this;
            
        CustomHandlersManager.RegisterEventsHandler(Events);
    }

    public override void Disable()
    {
        CustomHandlersManager.UnregisterEventsHandler(Events);

        Events = null;
            
        Instance = null;
    }
}