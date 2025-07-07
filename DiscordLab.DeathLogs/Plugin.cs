using DiscordLab.Bot.API.Attributes;
using LabApi.Features;
using DiscordLab.Dependency;

namespace DiscordLab.DeathLogs
{
    public class Plugin : Plugin<Config, Translation>
    {
        public static Plugin Instance;
        
        public override string Name { get; } = "DiscordLab.DeathLogs";
        public override string Description { get; } = "Adds death logging capabilities";
        public override string Author { get; } = "LumiFae";
        public override Version Version { get; } = typeof(Plugin).Assembly.GetName().Version;
        public override Version RequiredApiVersion { get; } = new(LabApiProperties.CompiledVersion);
        
        public override void Enable()
        {
            Instance = this;
            
            CallOnLoadAttribute.Load();
        }

        public override void Disable()
        {
            CallOnUnloadAttribute.Unload();
            
            Instance = null;
        }
    }
}