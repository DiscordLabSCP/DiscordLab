using System.ComponentModel;
using Discord;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Interfaces;

namespace DiscordLab.XPSystem
{
    public class Config : IConfig, IDLConfig
    {
        [Description(DescriptionConstants.IsEnabled)]
        public bool IsEnabled { get; set; } = true;
        [Description(DescriptionConstants.Debug)]
        public bool Debug { get; set; } = false;

        [Description("The channel ID to send the level up messages to.")]
        public ulong ChannelId { get; set; } = new();

        [Description("The hex color code of the embed. Do not include the #.")]
        public string Color { get; set; } = "3498DB";
        
        [Description(DescriptionConstants.GuildId)]
        public ulong GuildId { get; set; }
    }
}