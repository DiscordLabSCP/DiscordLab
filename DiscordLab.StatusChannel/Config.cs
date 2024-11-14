using System.ComponentModel;
using Exiled.API.Interfaces;

namespace DiscordLab.StatusChannel;

public class Config : IConfig
{
    public bool IsEnabled { get; set; } = true;
    public bool Debug { get; set; } = false;
    [Description("The channel ID where the status message will be sent.")]
    public ulong ChannelId { get; set; } = new ();
    [Description("Do not set this yourself, this will be set by the bot on first run.")]
    public ulong MessageId { get; set; } = new ();
}