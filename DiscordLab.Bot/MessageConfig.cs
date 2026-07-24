using System.ComponentModel;

namespace DiscordLab.Bot;

public class MessageConfig
{
    [Description("Do not edit unless you know what you are doing")]
    public Dictionary<string, ulong> MessageIds = new();
}