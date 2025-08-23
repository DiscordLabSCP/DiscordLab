using System.ComponentModel;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Features.Embed;

namespace DiscordLab.DeathLogs;

public class Translation
{
    [Description("The message that will be sent when a player dies.")]
    public MessageContent PlayerDeath { get; set; } =
        "`{target}` (`{targetrole}`) has been killed by `{player}` as `{playerrole}`. They died from: `{cause}`";

    [Description("The message that will be sent when a cuffed player dies, unless the cuffed channel is disabled.")]
    public MessageContent CuffedPlayerDeath { get; set; } =
        "`{target}` (`{targetrole}`) has been killed by `{player}` as `{playerrole}` while cuffed. They died from: `{cause}`";

    [Description(
        "The message that will be sent when a player dies by their own actions, or just they died because of something else.")]
    public MessageContent PlayerDeathSelf { get; set; } =
        "`{player}` (`{playerrole}`) has died. They died from: `{cause}`";

    [Description("The message that will be sent when a player dies due to someone on their own team.")]
    public MessageContent TeamKill { get; set; } =
        "`{target}` has been team-killed by `{player}`, they were both {role}. They died from: `{cause}`";

    [Description("The embed for when sending damage logs. Entries will be replaced with the entries below.")]
    public EmbedBuilder DamageLogEmbed { get; set; } = new()
    {
        Title = "Damage Logs",
        Description = "{entries}",
        Color = Discord.Color.Blue.ToString()
    };

    [Description("What each instance of damage will look like in the logs.")]
    public string DamageLogEntry { get; set; } =
        "{timetlong} | `{player}` did `{damage}` damage to `{target}` | Cause: `{cause}`";
}