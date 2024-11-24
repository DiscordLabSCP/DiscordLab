namespace DiscordLab.Bot.API.Features
{
    public class UpdateStatus
    {
        public string ModuleName { get; set; }
        public Version Version { get; set; }
        public string Url { get; set; }
    }
}