using System.ComponentModel;

namespace DiscordLab.Administration;

public class Config
{
    [Description("The channel to send server start logs")]
    public string ServerStartChannel { get; set; } = "default";

    [Description("Where server shutdown logs should be sent")]
    public string ServerShutdownChannel { get; set; } = "default";

    [Description("The channel to send error logs")]
    public string ErrorLogChannel { get; set; } = "default";

    [Description("The channel to send remote admin logs")]
    public string RemoteAdminChannel { get; set; } = "default";

    [Description("The channel to send normal command logs")]
    public string CommandLogChannel { get; set; } = "default";

    [Description("Should a secondary translation be used for remote admin commands whose response is a failure?")]
    public bool UseSecondaryTranslationRemoteAdmin { get; set; } = false;
    
    [Description("Should a secondary translation be used for normal commands whose response is a failure?")]
    public bool UseSecondaryTranslationCommand { get; set; } = false;

    [Description("Whether to add the commands to the bot. Is false then commands won't be used.")]
    public bool AddCommands { get; set; } = true;
}