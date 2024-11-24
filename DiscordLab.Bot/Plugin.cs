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
        public override Version Version => new (1, 3, 1);
        public override Version RequiredExiledVersion => new (8, 11, 0);
        public override PluginPriority Priority => PluginPriority.Higher;

        public static Plugin Instance { get; private set; }
        
        private HandlerLoader _handlerLoader;

        public override void OnEnabled()
        {
            Instance = this;
            
            _handlerLoader = new ();
            _handlerLoader.Load(Assembly);

            Task.Run(UpdateStatus.GetStatus);
            
            base.OnEnabled();
        }
        
        public override void OnDisabled()
        {
            _handlerLoader.Unload();
            _handlerLoader = null;
            
            SlashCommandLoader.ClearCommands();
            
            base.OnDisabled();
        }
    }
}