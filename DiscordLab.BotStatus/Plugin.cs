using DiscordLab.Bot.API.Interfaces;
using DiscordLab.Bot.API.Modules;
using Exiled.API.Enums;
using Exiled.API.Features;

namespace DiscordLab.BotStatus
{
    public class Plugin : Plugin<Config, Translation>
    {
        public override string Name => "DiscordLab.BotStatus";
        public override string Author => "JayXTQ";
        public override string Prefix => "DL.BotStatus";
        public override Version Version => new (1, 3, 0);
        public override Version RequiredExiledVersion => new (8, 11, 0);
        public override PluginPriority Priority => PluginPriority.Default;

        public static Plugin Instance { get; private set; }
        
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