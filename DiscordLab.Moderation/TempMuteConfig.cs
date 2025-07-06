namespace DiscordLab.Moderation
{
    public class TempMuteConfig
    {
        public Dictionary<string, DateTime> Mutes { get; set; } = new();
    }
}