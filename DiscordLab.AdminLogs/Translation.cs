using DiscordLab.Bot.API.Interfaces;

namespace DiscordLab.AdminLogs
{
    public class Translation : IConfig
    {
        public string Error { get; set; } = "Error on the server";
        
        public string ServerStart { get; set; } = "Server has started";
        
        public string ServerStartDescription { get; set; } = "The server has started and players can now connect.\nStarted {timer}";
    }
}