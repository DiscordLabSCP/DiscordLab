using System.ComponentModel;

namespace DiscordLab.RoundLogs
{
    public class Config
    {
        [Description("The guild ID, set to 0 for default guild.")]
        public ulong GuildId { get; set; } = 0;

        [Description("The channel to log to when someone's role changes.")]
        public ulong RoleChangeChannelId { get; set; } = 0;

        [Description("The channel to log to when someone swaps from an SCP to another.")]
        public ulong ScpSwapChannelId { get; set; } = 0;

        [Description("The channel to log to when NTF spawns.")]
        public ulong NtfSpawnChannelId { get; set; } = 0;

        [Description("The channel to log to when Chaos spawns.")]
        public ulong ChaosSpawnChannelId { get; set; } = 0;

        [Description("The channel to log to when someone gets cuffed.")]
        public ulong CuffedChannelId { get; set; } = 0;

        [Description("The channel to log to when someone gets uncuffed.")]
        public ulong UncuffedChannelId { get; set; } = 0;

        [Description("The channel to log to when the round starts.")]
        public ulong RoundStartedChannelId { get; set; } = 0;

        [Description("The channel to log to when the round ends.")]
        public ulong RoundEndedChannelId { get; set; } = 0;

        [Description("The channel to log to when decontamination starts.")]
        public ulong DecontaminationChannelId { get; set; } = 0;
    }
}