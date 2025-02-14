using DiscordLab.Bot.API.Interfaces;
using DiscordLab.Bot.API.Modules;
using LabApi.Features;

namespace DiscordLab.DeathLogs
{
    public class Plugin : LabApi.Loader.Features.Plugins.Plugin
    {
        public override string Name => "DiscordLab.DeathLogs";
        public override string Author => "LumiFae";
        public override string Description => "DeathLogs module for DiscordLab";
        public override Version Version => new (1, 5, 0);
        public override Version RequiredApiVersion => new (LabApiProperties.CompiledVersion);

        public static Plugin Instance { get; private set; } = null!;
        
        private HandlerLoader _handlerLoader = null!;
        
        public Config Config { get; set; } = null!;
        public Translation Translation { get; set; } = null!;

        public override void Enable()
        {
            Instance = this;
            
            _handlerLoader = new ();
            if(!_handlerLoader.Load()) throw new ("Failed to load module, contact us");
        }
        
        public override void Disable()
        {
            _handlerLoader.Unload();
            _handlerLoader = null!;
        }

        public override void LoadConfigs()
        {
            ConfigLoader.LoadConfigs(this, out Config config, out Translation translation);
            Config = config;
            Translation = translation;
        }
    }
}