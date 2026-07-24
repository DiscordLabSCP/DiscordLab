using DiscordLab.Core.API.Attributes;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;

namespace DiscordLab.Core;

public class Plugin : Plugin<Config>
{
    public static Plugin Instance { get; set; } = null!;
    
    public override string Name { get; } = "DiscordLab";
    public override string Description { get; } = "The core plugin for DiscordLab";
    public override string Author { get; } = "LumiFae";
    public override Version RequiredApiVersion { get; } = new(LabApiProperties.CompiledVersion);
    public override Version Version => field ??= GetType().Assembly.GetName().Version;

    public override void Enable()
    {
        Instance = this;
        
        CallOnLoadAttribute.Load();
    }

    public override void Disable()
    {
        Instance = null!;
        
        CallOnUnloadAttribute.Unload();
    }
}