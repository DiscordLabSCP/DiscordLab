using DiscordLab.AdvancedLogging.API.Modules;
using DiscordLab.AdvancedLogging.Handlers;
using DiscordLab.Bot.API.Modules;
using Exiled.API.Enums;
using Exiled.API.Features;
using UnityEngine;
using Log = DiscordLab.AdvancedLogging.API.Features.Log;

namespace DiscordLab.AdvancedLogging
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "DiscordLab.AdvancedLogging";
        public override string Author => "LumiFae";
        public override string Prefix => "DL.AdvancedLogging";
        public override Version Version => new (1, 4, 1);
        public override Version RequiredExiledVersion => new (8, 11, 0);
        public override PluginPriority Priority => PluginPriority.Low;

        public static Plugin Instance { get; private set; }
        
        private HandlerLoader _handlerLoader;

        public override void OnEnabled()
        {
            Instance = this;
            
            _handlerLoader = new ();
            
            if(!_handlerLoader.Load(Assembly)) return;
            
            EventManager.GetHandlers();
            
            foreach (Log log in DiscordBot.Instance.GetLogs())
            {
                Exiled.API.Features.Log.Debug($"Adding event handler for {log.Handler}.{log.Event}");
                EventManager.AddEventHandler(log.Handler, log.Event);
            }
            
            base.OnEnabled();
        }
        
        public override void OnDisabled()
        {
            _handlerLoader.Unload();

            _handlerLoader = null;
            
            EventManager.RemoveEventHandlers();
            
            base.OnDisabled();
        }
    }
}