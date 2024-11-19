using System.ComponentModel;
using Discord;
using Exiled.API.Interfaces;

namespace DiscordLab.XPSystem;

public class Config : IConfig
{
    public bool IsEnabled { get; set; } = true;
    public bool Debug { get; set; } = false;
    [Description("The channel ID to send the level up messages to.")]
    public ulong ChannelId { get; set; } = new ();
    [Description("The color of the embed.")]
    public Color Color { get; set; } = Color.Blue;
}