using System.ComponentModel;

namespace DiscordLab.StatusChannel
{
    public class MessageConfig
    {
        [Description("Do not set this, this will be set for you.")]
        public ulong MessageId { get; set; } = 0;
    }
}