using Exiled.API.Interfaces;

namespace DiscordLab.SCPSwap
{
    public class Translation : ITranslation
    {
        public string Message { get; set; } = "Player {player} ({playerid}) has swapped from {oldrole} to {newrole}.";
    }
}