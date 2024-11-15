using Discord.WebSocket;
using Exiled.API.Enums;
using Exiled.API.Features;

namespace DiscordLab.StatusLog
{
    public class Plugin : Plugin<Config, Translation>
    {
        public override string Name => "DiscordLab.StatusLog";
        public override string Author => "JayXTQ";
        public override string Prefix => "DL.SL";
        public override Version Version => new (1, 0, 0);
        public override Version RequiredExiledVersion => new (8, 11, 0);
        public override PluginPriority Priority => PluginPriority.Default;
        
        public static Plugin Instance { get; private set; }
        
        public SocketTextChannel Channel;
        
        private Events _events;
        
        public override void OnEnabled()
        {
            Bot.DiscordBot.ReadyEvent += Ready;
            Instance = this;
            _events = new();
            _events.Init();
            base.OnEnabled();
        }
        
        public override void OnDisabled()
        {
            Channel = null;
            _events.Unregister();
            _events = null;
            base.OnDisabled();
        }

        private void Ready()
        {
            if (Config.ChannelId == 0)
            {
                Log.Error("No channel ID is set.");
                return;
            }
            Channel = Bot.DiscordBot.Instance.Guild.GetTextChannel(Config.ChannelId);
        }
    }
}