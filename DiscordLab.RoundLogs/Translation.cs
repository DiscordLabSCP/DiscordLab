using Exiled.API.Interfaces;

namespace DiscordLab.RoundLogs
{
    public class Translation : ITranslation
    {
        public string RoundStartMessage { get; set; } = "Round has started! Started {timer}";
        public string RoundEndMessage { get; set; } = "Round has ended! Team {team} has won!";
        
        public string PlayerCuffedMessage { get; set; } = "{playernickname} has been cuffed by {issuernickname}!";
        public string PlayerUncuffedMessage { get; set; } = "{playernickname} has been uncuffed by {issuernickname}!";
        
        public string ChaosSpawnMessage { get; set; } = "Chaos Insurgency has spawned! {timer}";
        public string NtfSpawnMessage { get; set; } = "Nine-Tailed Fox ({unitname}-{unitnumber}) has spawned! {timer}";
    }
}