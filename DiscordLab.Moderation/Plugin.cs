using DiscordLab.Bot.API.Modules;
using LabApi.Features;
using LabApi.Loader.Features.Plugins.Enums;

namespace DiscordLab.Moderation
{
    public class Plugin : LabApi.Loader.Features.Plugins.Plugin
    {
        public override string Name => "DiscordLab.Moderation";
        public override string Author => "LumiFae";
        public override string Description => "Moderation module for DiscordLab";
        public override Version Version => new (1, 5, 0);
        public override Version RequiredApiVersion => new (LabApiProperties.CompiledVersion);
        public override LoadPriority Priority => LoadPriority.Low;

        public static Plugin Instance { get; private set; } = null!;
        
        private HandlerLoader _handlerLoader = null!;
        
        public Config Config { get; private set; } = null!;
        public Translation Translation { get; private set; } = null!;

        public override void Enable()
        {
            Instance = this;
            
            _handlerLoader = new ();
            if(!_handlerLoader.Load()) throw new ("Failed to load module, contact us.");
        }
        
        public override void Disable()
        {
            _handlerLoader.Unload();
            _handlerLoader = null!;
        }

        public override void LoadConfigs()
        {
            this.LoadConfigs(out Config config, out Translation translation);
            Config = config;
            Translation = translation;
        }
    }
}