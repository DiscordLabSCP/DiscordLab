using System.ComponentModel;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Interfaces;

namespace DiscordLab.AdminLogs
{
    public class Config : IConfig, IDLConfig
    {
        [Description(DescriptionConstants.IsEnabled)]
        public bool IsEnabled { get; set; } = true;
        [Description(DescriptionConstants.Debug)]
        public bool Debug { get; set; } = false;
        
        [Description("The channel where error logs will be sent.")]
        public ulong ErrorLogChannelId { get; set; } = new();
        
        [Description("The hex color code of the error logging embed. Do not add the #.")]
        public string ErrorLogColor { get; set; } = "3498DB";
        
        [Description("The channel where server start logs will be sent.")]
        public ulong ServerStartChannelId { get; set; } = new();
        
        [Description("The hex color code of the server start embed. Do not add the #.")]
        public string ServerStartColor { get; set; } = "3498DB";
        
        [Description(DescriptionConstants.GuildId)]
        public ulong GuildId { get; set; }
    }
}