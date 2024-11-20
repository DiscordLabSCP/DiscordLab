using DiscordLab.AdvancedLogging.API.Features;
using Exiled.API.Interfaces;

namespace DiscordLab.AdvancedLogging;

public class Config : IConfig
{
    public bool IsEnabled { get; set; } = true;
    public bool Debug { get; set; } = false;
    public PlayerConfig Player { get; set; } = new();
}