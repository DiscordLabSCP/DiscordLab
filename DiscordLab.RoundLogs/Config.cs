using System.ComponentModel;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Interfaces;

namespace DiscordLab.RoundLogs
{
    public class Config : IConfig, IDLConfig
    {
        [Description(DescriptionConstants.IsEnabled)]
        public bool IsEnabled { get; set; } = true;
        [Description(DescriptionConstants.Debug)]
        public bool Debug { get; set; } = false;
        
        [Description("The channel ID to send the round start messages to.")]
        public ulong RoundStartChannelId { get; set; } = new();
        [Description("The channel ID to send the round end messages to.")]
        public ulong RoundEndChannelId { get; set; } = new();
        [Description("The channel ID to send the cuff logs to. (When a player gets cuffed)")]
        public ulong CuffedChannelId { get; set; } = new();
        [Description("The channel ID to send the uncuff logs to. (When a player gets uncuffed)")]
        public ulong UncuffedChannelId { get; set; } = new();
        [Description("The channel ID to send the Chaos spawn logs to.")]
        public ulong ChaosSpawnChannelId { get; set; } = new();
        [Description("The channel ID to send the NTF spawn logs to.")]
        public ulong NtfSpawnChannelId { get; set; } = new();
        
        [Description(DescriptionConstants.GuildId)]
        public ulong GuildId { get; set; } = new();
    }
}