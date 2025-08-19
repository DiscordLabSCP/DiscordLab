using System.ComponentModel;
using DiscordLab.Bot.API.Features;

namespace DiscordLab.RoundLogs;

public class Translation
{
    public MessageContent ScpSwapLog { get; set; } = "Player {player} has swapped from {oldrole} to {newrole}.";

    public MessageContent RoleChangeLog { get; set; } = "Player {player} has swapped from {oldrole} to {newrole}. They were swapped because of {reason}";

    public MessageContent NtfSpawn { get; set; } = "NTF has respawned with the following players:\n{players}";

    public MessageContent ChaosSpawn { get; set; } = "Chaos has respawned with the following player:\n{players}";

    public string PlayerListItem { get; set; } = "- {player}";

    public MessageContent Cuffed { get; set; } = "Player {target} has been cuffed by {player}";

    public MessageContent Uncuffed { get; set; } = "Player {target} has been uncuffed by {player}";

    [Description("This doesn't come with players as that is available in DiscordLab.ConnectionLogs. Same applies to RoundEnd")]
    public MessageContent RoundStart { get; set; } = "Round has started.";

    public MessageContent RoundEnd { get; set; } = "Round has ended, {winner} has won the round.";

    public MessageContent Decontamination { get; set; } = "Decontamination has begun.";
}