using System.ComponentModel;
using Exiled.API.Interfaces;

namespace DiscordLab.BotStatus
{
    public class Config : IConfig
    {
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;

        [Description("Set the Discord bot's status to orange when the server is empty.")]
        public bool IdleOnEmpty { get; set; } = false;
    }
}