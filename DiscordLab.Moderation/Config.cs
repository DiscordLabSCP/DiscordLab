namespace DiscordLab.Moderation
{
    public class Config
    {
        public ulong GuildId { get; set; } = 0;

        public ulong MuteLogChannelId { get; set; } = 0;

        public ulong UnmuteLogChannelId { get; set; } = 0;

        public ulong BanLogChannelId { get; set; } = 0;

        public ulong UnbanLogChannelId { get; set; } = 0;

        public bool AddCommands { get; set; } = true;
    }
}