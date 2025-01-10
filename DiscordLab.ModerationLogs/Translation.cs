using System.ComponentModel;
using Exiled.API.Interfaces;

namespace DiscordLab.ModerationLogs
{
    public class Translation : ITranslation
    {
        public string Player { get; set; } = "Player";
        public string PlayerId { get; set; } = "Player ID";
        public string Reason { get; set; } = "Reason";
        public string Issuer { get; set; } = "Issuer";
        public string IssuerId { get; set; } = "Issuer ID";
        public string Target { get; set; } = "Target";
        public string TargetId { get; set; } = "Target ID";
        public string Reporter { get; set; } = "Reporter";
        public string ReporterId { get; set; } = "Reporter ID";
        public string Duration { get; set; } = "Duration";
        [Description("What will show as the message content for when a player is banned")]
        public string PlayerBannedContent { get; set; } = string.Empty;
        public string PlayerBanned { get; set; } = "Player banned";
        [Description("What will show as the message content for when a player is reported")]
        public string PlayerReportedContent { get; set; } = string.Empty;
        public string PlayerReported { get; set; } = "Player reported";
        [Description("What will show as the message content for when a player is kicked")]
        public string PlayerKickedContent { get; set; } = string.Empty;
        public string PlayerKicked { get; set; } = "Player kicked";
        [Description("What will show as the message content for when a player is unbanned")]
        public string PlayerUnbannedContent { get; set; } = string.Empty;
        public string PlayerUnbanned { get; set; } = "Player unbanned";
        [Description("What will show as the message content for when a player is muted")]
        public string PlayerMutedContent { get; set; } = string.Empty;
        public string PlayerMuted { get; set; } = "Player muted";
        [Description("What will show as the message content for when a player is unmuted")]
        public string PlayerMuteRevokedContent { get; set; } = string.Empty;
        public string PlayerMuteRevoked { get; set; } = "Player mute revoked";
        public string AdminChatMessage { get; set; } = "Admin chat message";
        public string RemoteAdminCommand { get; set; } = "Remote admin command";
        public string Command { get; set; } = "Command";
        public string Message { get; set; } = "Message";
    }
}