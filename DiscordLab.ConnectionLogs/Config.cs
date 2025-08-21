using System.ComponentModel;

namespace DiscordLab.ConnectionLogs;

public class Config
{
    [Description("The channel where the join logs will be sent.")]
    public ulong JoinChannelId { get; set; } = 0;

    [Description("The channel where the leave logs will be sent.")]
    public ulong LeaveChannelId { get; set; } = 0;

    [Description("The channel where the round start logs will be sent.")]
    public ulong RoundStartChannelId { get; set; } = 0;

    [Description("The channel where the round end logs will be sent. Optional.")]
    public ulong RoundEndChannelId { get; set; } = 0;

    public ulong GuildId { get; set; } = 0;
}