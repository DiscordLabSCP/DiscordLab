using Exiled.API.Features;

namespace DiscordLab.StatusChannel
{
    internal class Plugin : Plugin<Config, Translation>
    {
        public static Plugin Instance;
        public Discord Discord;
        private Events _events;
        
        public override void OnEnabled()
        {
            Instance = this;
            Discord = new();
            _events = new();
            _events.Init();
            Discord.Init();
            base.OnEnabled();
        }
        
        public override void OnDisabled()
        {
            Discord = null;
            _events.Unregister();
            _events = null;
            base.OnDisabled();
        }
    }
}