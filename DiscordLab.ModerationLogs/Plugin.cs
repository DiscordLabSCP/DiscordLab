using System.Globalization;
using DiscordLab.Bot.API.Modules;
using Exiled.API.Enums;
using Exiled.API.Features;

namespace DiscordLab.ModerationLogs
{
    public class Plugin : Plugin<Config, Translation>
    {
        public override string Name => "DiscordLab.ModerationLogs";
        public override string Author => "JayXTQ";
        public override string Prefix => "DL.ModerationLogs";
        public override Version Version => new (1, 4, 0);
        public override Version RequiredExiledVersion => new (8, 11, 0);
        public override PluginPriority Priority => PluginPriority.Default;

        public static Plugin Instance { get; private set; }
        
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
        
        public static uint GetColor(string color)
        {
            return uint.Parse(color, NumberStyles.HexNumber);
        }
    }
}