using System.ComponentModel;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Interfaces;

namespace DiscordLab.ConnectionLogs
{
    public class Config : IConfig, IDLConfig
    {
        [Description(DescriptionConstants.IsEnabled)]
        public bool IsEnabled { get; set; } = true;
        
        [Description(DescriptionConstants.Debug)]
        public bool Debug { get; set; } = false;

        [Description("The channel where the join logs will be sent.")]
        public ulong JoinChannelId { get; set; } = new();

        [Description("The channel where the leave logs will be sent.")]
        public ulong LeaveChannelId { get; set; } = new();

        [Description("The channel where the round start logs will be sent.")]
        public ulong RoundStartChannelId { get; set; } = new();
        
        [Description("The channel where the round end logs will be sent. Optional.")]
        public ulong RoundEndChannelId { get; set; } = new();
        
        [Description(DescriptionConstants.GuildId)]
        public ulong GuildId { get; set; }
    }
}