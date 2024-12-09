using System.ComponentModel;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Interfaces;

namespace DiscordLab.AdvancedLogging
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