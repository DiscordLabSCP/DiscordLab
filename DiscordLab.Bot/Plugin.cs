using DiscordLab.Bot.API.Modules;
using Exiled.API.Enums;
using Exiled.API.Features;

namespace DiscordLab.Bot
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "DiscordLab";
        public override string Author => "JayXTQ";
        public override string Prefix => "DiscordLab";
        public override Version Version => new (1, 3, 0);
        public override Version RequiredExiledVersion => new (8, 11, 0);
        public override PluginPriority Priority => PluginPriority.High;

        public static Plugin Instance { get; private set; }
        
        private HandlerLoader _handlerLoader;

        public override void OnEnabled()
        {
            Instance = this;
            
            _handlerLoader = new ();
            _handlerLoader.Load(Assembly);

            Task.Run(UpdateStatus.GetStatus);
            
            UpdateStatus.OnUpdateStatus += OnUpdateStatus;
            
            base.OnEnabled();
        }
        
        public override void OnDisabled()
        {
            _handlerLoader.Unload();
            _handlerLoader = null;
            
            UpdateStatus.OnUpdateStatus -= OnUpdateStatus;
            
            SlashCommandLoader.ClearCommands();
            
            base.OnDisabled();
        }

        private void OnUpdateStatus(List<API.Features.UpdateStatus> statuses)
        {
            API.Features.UpdateStatus status = statuses.FirstOrDefault(x => x.ModuleName == "DiscordLab.Bot");
            if (status == null)
            {
                return;
            }
            if(status.Version > Version)
            {
                Log.Warn($"There is a new version of DiscordLab.Bot available! Download it from {status.Url}");
            }
        }
    }
}