using System.ComponentModel;

namespace DiscordLab.Moderation;

public class Config
{
    public ulong GuildId { get; set; } = 0;

    public ulong MuteLogChannelId { get; set; } = 0;

    public ulong UnmuteLogChannelId { get; set; } = 0;

    public ulong BanLogChannelId { get; set; } = 0;

    public ulong UnbanLogChannelId { get; set; } = 0;

    public ulong AdminChatLogChannelId { get; set; } = 0;

    [Description("Whether to add the Discord slash commands.")]
    public bool AddCommands { get; set; } = true;

    [Description("Whether to enable the temp mute remote admin command.")]
    public bool AddTempMuteCommand { get; set; } = true;
}