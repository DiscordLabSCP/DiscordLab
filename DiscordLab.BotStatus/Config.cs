using System.ComponentModel;
using DiscordLab.Core.API.Enums;

namespace DiscordLab.BotStatus;

public class Config
{
    [Description("Whether Custom or Playing activity type")]
    public ActivityType ActivityType { get; set; } = ActivityType.Custom;
    
    public bool IdleOnEmpty { get; set; } = false;
}