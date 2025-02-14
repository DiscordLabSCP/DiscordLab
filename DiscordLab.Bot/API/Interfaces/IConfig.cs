namespace DiscordLab.Bot.API.Interfaces
{
    public interface IConfig
    {
        public bool IsEnabled { get; set; }
        public bool Debug { get; set; }
    }
}