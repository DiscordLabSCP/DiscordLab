using System.ComponentModel;
using Exiled.API.Interfaces;

namespace DiscordLab.ConnectionLogs;

public class Config : IConfig
{
    public bool IsEnabled { get; set; } = true;
    public bool Debug { get; set; } = false;
    [Description("The channel where the join logs will be sent.")]
    public ulong JoinChannelId { get; set; } = new ();
    [Description("The channel where the leave logs will be sent.")]
    public ulong LeaveChannelId { get; set; } = new ();
    [Description("The channel where the round start logs will be sent.")]
    public ulong RoundStartChannelId { get; set; } = new ();
}