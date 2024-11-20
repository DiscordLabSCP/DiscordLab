using Discord.WebSocket;

namespace DiscordLab.AdvancedLogging.API.Features;

public class Channel
{
    public string Handler { get; set; }
    public string EventName { get; set; }
    public ulong ChannelId { get; set; }
    public SocketTextChannel TextChannel { get; set; }
}