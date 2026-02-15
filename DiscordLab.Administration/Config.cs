using System.ComponentModel;

namespace DiscordLab.Administration;

public class Config
{
    public ulong GuildId { get; set; } = 0;

    [Description("The channel to send server start logs")]
    public ulong ServerStartChannelId { get; set; } = 0;

    [Description("Where server shutdown logs should be sent")]
    public ulong ServerShutdownChannelId { get; set; } = 0;

    [Description("The channel to send error logs")]
    public ulong ErrorLogChannelId { get; set; } = 0;

    [Description("The channel to send remote admin logs")]
    public ulong RemoteAdminChannelId { get; set; } = 0;

    [Description("The channel to send normal command logs")]
    public ulong CommandLogChannelId { get; set; } = 0;

    [Description("Whether to add the commands to the bot. Is false then commands won't be used.")]
    public bool AddCommands { get; set; } = true;

    [Description("Should a secondary translation be used for remote admin commands whose response is a failure?")]
    public bool UseSecondaryTranslationRemoteAdmin { get; set; } = false;
    
    [Description("Should a secondary translation be used for normal commands whose response is a failiure?")]
    public bool UseSecondaryTranslationCommand { get; set; } = false;
}