using System.ComponentModel;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Interfaces;

namespace DiscordLab.ModerationLogs
{
    public class Config : IConfig, IDLConfig
    {
        [Description(DescriptionConstants.IsEnabled)]
        public bool IsEnabled { get; set; } = true;
        [Description(DescriptionConstants.Debug)]
        public bool Debug { get; set; } = false;

        [Description("The channel where the ban logs will be sent.")]
        public ulong BanChannelId { get; set; } = new();

        [Description("The hex color code of the ban embed. Do not add the #.")]
        public string BanColor { get; set; } = "3498DB";

        [Description("The channel where the unban logs will be sent.")]
        public ulong UnbanChannelId { get; set; } = new();

        [Description("The hex color code of the unban embed. Do not add the #.")]
        public string UnbanColor { get; set; } = "3498DB";

        [Description("The channel where the kick logs will be sent.")]
        public ulong KickChannelId { get; set; } = new();

        [Description("The hex color code of the kick embed. Do not add the #.")]
        public string KickColor { get; set; } = "3498DB";

        [Description("The channel where the mute logs will be sent.")]
        public ulong MuteChannelId { get; set; } = new();

        [Description("The hex color code of the mute embed. Do not add the #.")]
        public string MuteColor { get; set; } = "3498DB";

        [Description("The channel where the unmute logs will be sent.")]
        public ulong UnmuteChannelId { get; set; } = new();

        [Description("The hex color code of the unmute embed. Do not add the #.")]
        public string UnmuteColor { get; set; } = "3498DB";

        [Description("The channel where the warn logs will be sent.")]
        public ulong AdminChatChannelId { get; set; } = new();

        [Description("The hex color code of the admin chat embed. Do not add the #.")]
        public string AdminChatColor { get; set; } = "3498DB";

        [Description("The channel where reports will be sent.")]
        public ulong ReportChannelId { get; set; } = new();

        [Description("The hex color code of the report embed. Do not add the #.")]
        public string ReportColor { get; set; } = "3498DB";
        
        [Description("The channel where remote admin logs will be sent.")]
        public ulong RemoteAdminChannelId { get; set; } = new();
        
        [Description("The hex color code of the remote admin embed. Do not add the #.")]
        public string RemoteAdminColor { get; set; } = "3498DB";
        
        [Description(DescriptionConstants.GuildId)]
        public ulong GuildId { get; set; }
    }
}