using System.ComponentModel;
using Exiled.API.Interfaces;

namespace DiscordLab.SCPSwap
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
        [Description("The channel where the swap logs will be sent.")]
        public ulong ChannelId { get; set; } = new();
    }
}