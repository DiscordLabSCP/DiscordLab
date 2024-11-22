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
        public string PlayerBanned { get; set; } = "Player banned";
        public string PlayerKicked { get; set; } = "Player kicked";
        public string PlayerUnbanned { get; set; } = "Player unbanned";
        public string PlayerMuted { get; set; } = "Player muted";
        public string PlayerMuteRevoked { get; set; } = "Player mute revoked";
        public string AdminChatMessage { get; set; } = "Admin chat message";
        public string Message { get; set; } = "Message";
    }
}