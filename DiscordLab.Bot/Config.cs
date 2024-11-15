using System.ComponentModel;
using Exiled.API.Interfaces;

namespace DiscordLab.Bot;

public class Config : IConfig
{
    public bool IsEnabled { get; set; } = true;
    public bool Debug { get; set; } = false;
    [Description("The token of the bot.")]
    public string Token { get; set; } = "token";
    [Description("The guild where the bot will be used.")]
    public ulong GuildId { get; set; } = new ();
}