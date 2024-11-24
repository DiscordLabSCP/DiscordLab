using DiscordLab.Bot.API.Modules;
using Exiled.API.Enums;
using Exiled.API.Features;
using Exiled.Loader;

namespace DiscordLab.Moderation
{
    public class Plugin : Plugin<Config, Translation>
    {
        public override string Name => "DiscordLab.Moderation";
        public override string Author => "JayXTQ";
        public override string Prefix => "DL.Moderation";
        public override Version Version => new (1, 3, 0);
        public override Version RequiredExiledVersion => new (8, 11, 0);
        public override PluginPriority Priority => PluginPriority.Default;

        public static Plugin Instance { get; private set; }
        
        private bool ModerationLogsEnabled { get; set; }
        
        private HandlerLoader _handlerLoader;

        public override void OnEnabled()
        {
            Instance = this;
            
            _handlerLoader = new ();
            _handlerLoader.Load(Assembly);
            
            base.OnEnabled();
        }
        
        public override void OnDisabled()
        {
            _handlerLoader.Unload();
            _handlerLoader = null;
            
            base.OnDisabled();
        }

        public bool CheckModerationLogsEnabled()
        {
            if (!ModerationLogsEnabled)
            {
                ModerationLogsEnabled = Loader.Plugins.FirstOrDefault(p => p.Name == "DiscordLab.ModerationLogs" && p.Config.IsEnabled) != null;
            }
            return ModerationLogsEnabled;
        }
    }
}