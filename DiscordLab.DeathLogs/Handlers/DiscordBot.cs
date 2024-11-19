using Discord.WebSocket;
using DiscordLab.Bot.API.Interfaces;

namespace DiscordLab.DeathLogs.Handlers;

public class DiscordBot : IRegisterable
{
    public static DiscordBot Instance { get; private set; }
    
    private SocketTextChannel Channel { get; set; }
    private SocketTextChannel CuffedChannel { get; set; }
    private SocketTextChannel SelfChannel { get; set; }
    
    public void Init()
    {
        Instance = this;
    }
    
    public void Unregister()
    {
        Channel = null;
    }

    public SocketTextChannel GetChannel()
    {
        if(Bot.Handlers.DiscordBot.Instance.Guild == null) return null;
        return Channel ??= Bot.Handlers.DiscordBot.Instance.Guild.GetTextChannel(Plugin.Instance.Config.ChannelId);
    }
    
    public SocketTextChannel GetCuffedChannel()
    {
        if(Bot.Handlers.DiscordBot.Instance.Guild == null) return null;
        return CuffedChannel ??= Bot.Handlers.DiscordBot.Instance.Guild.GetTextChannel(Plugin.Instance.Config.CuffedChannelId);
    }

    public SocketTextChannel GetSelfChannel()
    {
        if(Bot.Handlers.DiscordBot.Instance.Guild == null) return null;
        return SelfChannel ??= Bot.Handlers.DiscordBot.Instance.Guild.GetTextChannel(Plugin.Instance.Config.SelfChannelId);
    }
}