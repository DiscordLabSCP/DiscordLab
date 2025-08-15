using System.ComponentModel;

namespace DiscordLab.RoundLogs;

public class Translation
{
    public string ScpSwapLog { get; set; } = "Player {player} has swapped from {oldrole} to {newrole}.";

    public string RoleChangeLog { get; set; } = "Player {player} has swapped from {oldrole} to {newrole}. They were swapped because of {reason}";

    public string NtfSpawn { get; set; } = "NTF has respawned with the following players:\n{players}";

    public string ChaosSpawn { get; set; } = "Chaos has respawned with the following player:\n{players}";

    public string PlayerListItem { get; set; } = "- {player}";

    public string Cuffed { get; set; } = "Player {target} has been cuffed by {player}";

    public string Uncuffed { get; set; } = "Player {target} has been uncuffed by {player}";

    [Description("This doesn't come with players as that is available in DiscordLab.ConnectionLogs. Same applies to RoundEnd")]
    public string RoundStart { get; set; } = "Round has started.";

    public string RoundEnd { get; set; } = "Round has ended, {winner} has won the round.";

    public string Decontamination { get; set; } = "Decontamination has begun.";
}