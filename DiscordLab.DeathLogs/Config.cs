using System.ComponentModel;
using Exiled.API.Interfaces;

namespace DiscordLab.DeathLogs;

public class Config : IConfig
{
    public bool IsEnabled { get; set; } = true;
    public bool Debug { get; set; } = false;
    [Description("The channel where the normal death logs will be sent.")]
    public ulong ChannelId { get; set; } = new ();
    [Description("The channel where the death logs of cuffed players will be sent. Keep as default value to disable. Disabling this will make it so logs are only sent to the normal death logs channel, but without the cuffed identifier.")]
    public ulong CuffedChannelId { get; set; } = new ();
}