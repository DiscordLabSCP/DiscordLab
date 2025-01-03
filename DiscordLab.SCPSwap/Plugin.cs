﻿using System.Globalization;
using DiscordLab.Bot.API.Modules;
using Exiled.API.Enums;
using Exiled.API.Features;

namespace DiscordLab.SCPSwap
{
    public class Plugin : Plugin<Config, Translation>
    {
        public override string Name => "DiscordLab.SCPSwap";
        public override string Author => "LumiFae";
        public override string Prefix => "DL.SCPSwap";
        public override Version Version => new (1, 5, 0);
        public override Version RequiredExiledVersion => new (9, 0, 0);
        public override PluginPriority Priority => PluginPriority.Default;

        public static Plugin Instance { get; private set; }
        
        private HandlerLoader _handlerLoader;

        public override void OnEnabled()
        {
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