using Exiled.API.Interfaces;

namespace DiscordLab.ConnectionLogs;

public class Config : IConfig
{
    public bool IsEnabled { get; set; } = true;
    public bool Debug { get; set; } = false;
    public ulong JoinChannelId { get; set; } = new ();
    public ulong LeaveChannelId { get; set; } = new ();
    public ulong RoundStartChannelId { get; set; } = new ();
}