using Discord;
using Discord.Rest;
using Discord.WebSocket;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Features;
using Newtonsoft.Json.Linq;

namespace DiscordLab.ConnectionLogs.Handlers;

public class DiscordBot : IRegisterable
{
    public static DiscordBot Instance { get; private set; }
    
    private SocketTextChannel JoinChannel { get; set; }
    private SocketTextChannel LeaveChannel { get; set; }
    private SocketTextChannel RoundStartChannel { get; set; }
    
    public void Init()
    {
        Instance = this;
    }
    
    public void Unregister()
    {
        JoinChannel = null;
        LeaveChannel = null;
        RoundStartChannel = null;
    }

    public SocketTextChannel GetJoinChannel()
    {
        if(Bot.Handlers.DiscordBot.Instance.Guild == null) return null;
        return JoinChannel ??= Bot.Handlers.DiscordBot.Instance.Guild.GetTextChannel(Plugin.Instance.Config.JoinChannelId);
    }
    
    public SocketTextChannel GetLeaveChannel()
    {
        if(Bot.Handlers.DiscordBot.Instance.Guild == null) return null;
        return LeaveChannel ??= Bot.Handlers.DiscordBot.Instance.Guild.GetTextChannel(Plugin.Instance.Config.LeaveChannelId);
    }

    public SocketTextChannel GetRoundStartChannel()
    {
        if(Bot.Handlers.DiscordBot.Instance.Guild == null) return null;
        return RoundStartChannel ??= Bot.Handlers.DiscordBot.Instance.Guild.GetTextChannel(Plugin.Instance.Config.RoundStartChannelId);
    }
}