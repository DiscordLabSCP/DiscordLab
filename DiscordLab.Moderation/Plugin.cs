using DiscordLab.Bot.API.Modules;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Loader;

namespace DiscordLab.Moderation
{
    public class Plugin : Plugin<Config, Translation>
    {
        public override string Name => "DiscordLab.Moderation";
        public override string Author => "LumiFae";
        public override string Prefix => "DL.Moderation";
        public override Version Version => new (1, 4, 1);
        public override Version RequiredExiledVersion => new (8, 11, 0);
        public override PluginPriority Priority => PluginPriority.Low;

        public static Plugin Instance { get; private set; }
        
        private bool ModerationLogsEnabled { get; set; }
        private object ModerationLogsHandler { get; set; }
        
        private HandlerLoader _handlerLoader;

        public override void OnEnabled()
        {
            Instance = this;
            
            _handlerLoader = new ();
            if(!_handlerLoader.Load(Assembly)) return;
            
            base.OnEnabled();
        }
        
        public override void OnDisabled()
        {
            _handlerLoader.Unload();
            _handlerLoader = null;
            
            base.OnDisabled();
        }
    }
}