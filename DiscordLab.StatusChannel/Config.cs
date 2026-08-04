using System.ComponentModel;

namespace DiscordLab.StatusChannel;

public class Config
{
    [Description("The channel that you want the message sent to.")]
    public string Channel { get; set; } = "default";

    public bool AddCommand { get; set; } = true;
}