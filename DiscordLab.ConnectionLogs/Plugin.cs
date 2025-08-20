using DiscordLab.Bot.API.Features;
using DiscordLab.Dependency;
using LabApi.Events.CustomHandlers;
using LabApi.Features;

namespace DiscordLab.ConnectionLogs;

public class Plugin : Plugin<Config, Translation>
{
    public static Plugin Instance;
        
    public override string Name { get; } = "DiscordLab.ConnectionLogs";
    public override string Description { get; } = "Adds logging for connection based information";
    public override string Author { get; } = "LumiFae";
    public override Version Version => GetType().Assembly.GetName().Version;
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