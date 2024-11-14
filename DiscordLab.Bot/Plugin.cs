using Exiled.API.Features;

namespace DiscordLab.Bot
{
    public class Plugin : Plugin<Config>
    {
        public static Plugin Instance;
        
        private Discord _discord;

        public override void OnEnabled()
        {
            Instance = this;
            _discord = new ();
            _discord.Init();
            base.OnEnabled();
        }
        
        public override void OnDisabled()
        {
            _discord.Unregister();
            _discord = null;
            base.OnDisabled();
        }
    }
}