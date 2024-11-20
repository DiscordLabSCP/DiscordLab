using DiscordLab.AdvancedLogging.API.Features;
using Exiled.API.Interfaces;

namespace DiscordLab.AdvancedLogging;

public class Translation : ITranslation
{
    public PlayerTranslation Player { get; set; } = new();
}