namespace DiscordLab.BotStatus;

public class Translation
{
    public string EmptyContent { get; set; } = "0/{maxplayers} players online";

    public string NormalContent { get; set; } = "{playercount}/{maxplayers} players online.";
}