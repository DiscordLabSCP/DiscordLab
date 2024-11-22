using System.ComponentModel;
using Discord;
using Exiled.API.Interfaces;

namespace DiscordLab.XPSystem
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("The channel ID to send the level up messages to.")]
        public ulong ChannelId { get; set; } = new();

        [Description("The hex color code of the embed. Do not include the #.")]
        public string Color { get; set; } = "3498DB";
    }
}