using System.ComponentModel;
using DiscordLab.Core;
using DiscordLab.Core.API.Commands;

namespace DiscordLab.Bot;

public class Translation
{
    [Description("Here {code} is required as it gives the code that the user needs to put in. nameparsed is your server name without any of the formatting.")]
    public MessageContent DiscordResponse { get; set; } = "Please run .discordlink {code} in SCP: Secret Laboratory on the {nameparsed} server";
}