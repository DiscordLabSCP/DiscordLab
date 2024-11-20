using Exiled.API.Features;

namespace DiscordLab.AdvancedLogging.API.Features;

public class HurtQueueItem
{
    public Player Player { get; set; }
    public List<HurtQueueItemAttacker> Attackers { get; set; } = new();
}

public class HurtQueueItemAttacker
{
    public Player Attacker { get; set; }
    public float Damage { get; set; }
}