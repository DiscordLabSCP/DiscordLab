using Discord.WebSocket;

namespace DiscordLab.AdvancedLogging.API.Features;

public class ChannelType
{
    public string Handler { get; set; }
    public string Event { get; set; }
    public SocketTextChannel Channel { get; set; }
    public ulong ChannelId { get; set; }
}