using System.ComponentModel;

namespace DiscordLab.StatusChannel;

public class Config
{
    [Description("The channel that you want the message sent to.")]
    public ulong ChannelId { get; set; } = 0;

    [Description("The guild ID, set to 0 for default guild.")]
    public ulong GuildId { get; set; } = 0;

    public bool AddCommand { get; set; } = true;
}