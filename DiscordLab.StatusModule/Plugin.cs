﻿using Exiled.API.Enums;
using Exiled.API.Features;

namespace DiscordLab.StatusModule
{
    public class Plugin : Plugin<Config, Translation>
    {
        public override string Name => "DiscordLab.StatusModule";
        public override string Author => "JayXTQ";
        public override string Prefix => "DL.SM";
        public override Version Version => new (1, 0, 0);
        public override Version RequiredExiledVersion => new (8, 11, 0);
        public override PluginPriority Priority => PluginPriority.Default;
        
        public static Plugin Instance { get; private set; }
        public DiscordBot Discord;
        private Events _events;
        
        public override void OnEnabled()
        {
            Instance = this;
            Discord = new();
            Discord.Init();
            _events = new();
            _events.Init();
            base.OnEnabled();
        }
        
        public override void OnDisabled()
        {
            Discord.Unregister();
            Discord = null;
            _events.Unregister();
            _events = null;
            base.OnDisabled();
        }
    }
}