﻿using DiscordLab.Bot.API.Interfaces;
using DiscordLab.Bot.API.Modules;
using Exiled.API.Enums;
using Exiled.API.Features;
using XPSystem;

namespace DiscordLab.XPSystem
{
    public class Plugin : Plugin<Config, Translation>
    {
        public override string Name => "DiscordLab.XPSystem";
        public override string Author => "LumiFae";
        public override string Prefix => "DL.XPSystem";
        public override Version Version => new (1, 5, 0);
        public override Version RequiredExiledVersion => new (8, 11, 0);
        public override PluginPriority Priority => PluginPriority.Low;

        public static Plugin Instance { get; private set; }
        
        private HandlerLoader _handlerLoader;

        public override void OnEnabled()
        {
            if (Main.Instance.Version < new Version(2, 0, 8))
            {
                Log.Error("DiscordLab.XPSystem requires XPSystem to be at least version 2.0.8!");
                return;
            }

            Instance = this;
            
            _handlerLoader = new ();
            
            if(!_handlerLoader.Load(Assembly)) return;
            
            base.OnEnabled();
        }
        
        public override void OnDisabled()
        {
            _handlerLoader.Unload();

            _handlerLoader = null;
            
            base.OnDisabled();
        }
    }
}