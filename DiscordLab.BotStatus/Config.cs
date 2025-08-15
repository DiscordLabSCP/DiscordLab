using Discord;

namespace DiscordLab.BotStatus;

public class Config
{
    public ActivityType ActivityType { get; set; } = ActivityType.CustomStatus;

    public bool IdleOnEmpty { get; set; } = false;
}