using System.ComponentModel;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Interfaces;

namespace DiscordLab.SCPSwap
{
    public class Config : IConfig, IDLConfig
    {
        [Description(DescriptionConstants.IsEnabled)]
        public bool IsEnabled { get; set; } = true;
        [Description(DescriptionConstants.Debug)]
        public bool Debug { get; set; } = false;
        [Description("The channel where the swap logs will be sent.")]
        public ulong ChannelId { get; set; } = new();
        [Description(DescriptionConstants.GuildId)]
        public ulong GuildId { get; set; }
    }
}