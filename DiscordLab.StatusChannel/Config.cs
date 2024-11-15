using Exiled.API.Interfaces;

namespace DiscordLab.StatusChannel;

public class Config : IConfig
{
    public bool IsEnabled { get; set; } = true;
    public bool Debug { get; set; } = false;
    public ulong ChannelId { get; set; } = new ();
}