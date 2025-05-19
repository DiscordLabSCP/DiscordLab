using System.ComponentModel;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Interfaces;

namespace DiscordLab.Moderation
{
    public class Config : IConfig, IDLConfig
    {
        [Description(DescriptionConstants.IsEnabled)]
        public bool IsEnabled { get; set; } = true;
        [Description(DescriptionConstants.Debug)]
        public bool Debug { get; set; } = false;
        
        [Description("The role ID for the role that is able to run this command")]
        public ulong BanCommandRole { get; set; } = 0;
        [Description("The role ID for the role that is able to run this command")]
        public ulong UnbanCommandRole { get; set; } = 0;
        [Description("The role ID for the role that is able to run this command")]
        public ulong SendCommandRole { get; set; } = 0;
        
        [Description(DescriptionConstants.GuildId)]
        public ulong GuildId { get; set; }
    }
}