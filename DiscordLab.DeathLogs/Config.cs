﻿using System.ComponentModel;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Interfaces;

namespace DiscordLab.DeathLogs
{
    public class Config : IConfig, IDLConfig
    {
        [Description(DescriptionConstants.IsEnabled)]
        public bool IsEnabled { get; set; } = true;
        [Description(DescriptionConstants.Debug)]
        public bool Debug { get; set; } = false;

        [Description("The channel where the normal death logs will be sent.")]
        public ulong ChannelId { get; set; } = new();

        [Description(
            "The channel where the death logs of cuffed players will be sent. Keep as default value to disable. Disabling this will make it so logs are only sent to the normal death logs channel, but without the cuffed identifier.")]
        public ulong CuffedChannelId { get; set; } = new();

        [Description(
            "The channel where logs will be sent when a player dies by their own actions, or just they died because of something else.")]
        public ulong SelfChannelId { get; set; } = new();

        [Description("The channel where logs will be sent when a player dies by a teamkill.")]
        public ulong TeamKillChannelId { get; set; } = new();
        
        [Description("If this is true, then the plugin will ignore the cuff state of the player and send the death logs to the normal death logs channel.")]
        public bool ScpIgnoreCuffed { get; set; } = true;
        
        [Description(DescriptionConstants.GuildId)]
        public ulong GuildId { get; set; }
    }
}