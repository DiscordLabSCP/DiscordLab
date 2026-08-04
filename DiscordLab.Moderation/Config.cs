using System.ComponentModel;

namespace DiscordLab.Moderation;

public class Config
{
    public string MuteLogChannel { get; set; } = "default";

    public string UnmuteLogChannel { get; set; } = "default";

    public string BanLogChannel { get; set; } = "default";

    public string UnbanLogChannel { get; set; } = "default";

    public string AdminChatLogChannel { get; set; } = "default";

    [Description("Whether to add the Discord slash commands.")]
    public bool AddCommands { get; set; } = true;

    [Description("Whether to enable the temp mute remote admin command.")]
    public bool AddTempMuteCommand { get; set; } = true;
}