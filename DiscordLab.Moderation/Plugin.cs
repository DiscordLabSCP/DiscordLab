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
            
            UpdateStatus.OnUpdateStatus += OnUpdateStatus;
            
            base.OnEnabled();
        }
        
        public override void OnDisabled()
        {
            _handlerLoader.Unload();
            _handlerLoader = null;
            
            UpdateStatus.OnUpdateStatus -= OnUpdateStatus;
            
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
        
        private void OnUpdateStatus(List<Bot.API.Features.UpdateStatus> statuses)
        {
            Bot.API.Features.UpdateStatus status = statuses.FirstOrDefault(x => x.ModuleName == Name);
            if (status == null)
            {
                return;
            }
            if(status.Version > Version)
            {
                Log.Warn($"There is a new version of {Name} available! Download it from {status.Url}");
            }
        }
    }
}