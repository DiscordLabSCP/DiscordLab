﻿using System.ComponentModel;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Interfaces;

namespace DiscordLab.Moderation
{
    public class Config : IConfig, IDLConfig
    {
        [Description(DescriptionConstants.IsEnabled)]
        public bool IsEnabled { get; set; } = true;
        [Description(DescriptionConstants.Debug)]
        public bool Debug { get; set; } = false;
        
        [Description(DescriptionConstants.GuildId)]
        public ulong GuildId { get; set; }
    }
}