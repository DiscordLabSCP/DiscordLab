using JetBrains.Annotations;

namespace DiscordLab.AdvancedLogging.API.Features
{
    public class Log
    {
        public string Handler { get; set; }
        public string Event { get; set; }
        public string Content { get; set; }
        [CanBeNull] public string Nullables { get; set; }
        public ulong ChannelId { get; set; }
    }
}