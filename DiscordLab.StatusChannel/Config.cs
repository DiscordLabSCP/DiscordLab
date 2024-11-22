using System.ComponentModel;
using Discord;
using Exiled.API.Interfaces;

namespace DiscordLab.StatusChannel
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("The channel where the status message will be sent / edited.")]
        public ulong ChannelId { get; set; } = new();

        [Description("The hex color code of the embed. Do not add the #.")]
        public string Color { get; set; } = "3498DB";
    }
}