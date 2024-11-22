using Discord.WebSocket;
using DiscordLab.Bot.API.Interfaces;

namespace DiscordLab.XPSystem.Handlers
{

    public class DiscordBot : IRegisterable
    {
        private static Translation Translation => Plugin.Instance.Translation;

        public static DiscordBot Instance { get; private set; }

        private SocketTextChannel Channel { get; set; }

        public void Init()
        {
            Instance = this;
        }

        public void Unregister()
        {
            Channel = null;
        }

        public SocketTextChannel GetChannel()
        {
            if (Bot.Handlers.DiscordBot.Instance.Guild == null) return null;
            if (Plugin.Instance.Config.ChannelId == 0) return null;
            return Channel ??= Bot.Handlers.DiscordBot.Instance.Guild.GetTextChannel(Plugin.Instance.Config.ChannelId);
        }
    }
}