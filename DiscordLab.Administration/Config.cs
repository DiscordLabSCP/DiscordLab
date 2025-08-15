using System.ComponentModel;

namespace DiscordLab.Administration;

public class Config
{
    public ulong GuildId { get; set; } = 0;

    [Description("The channel to send server start logs")]
    public ulong ServerStartChannelId { get; set; } = 0;

    [Description("The channel to send error logs")]
    public ulong ErrorLogChannelId { get; set; } = 0;

    [Description("The channel to send remote admin logs")]
    public ulong RemoteAdminChannelId { get; set; } = 0;

    [Description("The channel to send normal command logs")]
    public ulong CommandLogChannelId { get; set; } = 0;

    [Description("Whether to add the commands to the bot. Is false then commands won't be used.")]
    public bool AddCommands { get; set; } = true;
}