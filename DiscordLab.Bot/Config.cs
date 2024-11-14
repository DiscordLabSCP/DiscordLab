using Exiled.API.Interfaces;

namespace DiscordLab.Bot;

public class Config : IConfig
{
    public bool IsEnabled { get; set; } = true;
    public bool Debug { get; set; } = false;
    public string Token { get; set; } = "token";
}