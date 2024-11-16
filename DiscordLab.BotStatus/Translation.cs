using System.ComponentModel;
using Exiled.API.Interfaces;

namespace DiscordLab.BotStatus;

public class Translation : ITranslation
{
    [Description("The message that will be sent when the match is on-going.")]
    public string StatusMessage { get; set; } = "{current}/{max} currently online";
}