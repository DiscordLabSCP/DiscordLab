using System.ComponentModel;
using DiscordLab.Bot.API.Features;
using Exiled.API.Interfaces;

namespace DiscordLab.BotStatus
{
    public class Config : IConfig
    {
        [Description(DescriptionConstants.IsEnabled)]
        public bool IsEnabled { get; set; } = true;
        [Description(DescriptionConstants.Debug)]
        public bool Debug { get; set; } = false;

        [Description("Set the Discord bot's status to orange when the server is empty.")]
        public bool IdleOnEmpty { get; set; } = false;
    }
}