using System.ComponentModel;
using DiscordLab.Bot.API.Interfaces;

namespace DiscordLab.BotStatus
{
    public class Translation : IConfig
    {
        [Description("The message that will be sent when the match is on-going.")]
        public string StatusMessage { get; set; } = "{current}/{max} currently online";

        [Description("The message that will be sent when the server is empty.")]
        public string EmptyServer { get; set; } = "0/{max} currently online";
    }
}