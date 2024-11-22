using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Features;
using JetBrains.Annotations;

namespace DiscordLab.ModerationLogs.Handlers;

public class DiscordBot : IRegisterable
{
    private static Translation Translation => Plugin.Instance.Translation;
    
    public static DiscordBot Instance { get; private set; }
    
    private SocketTextChannel BanChannel { get; set; }
    private SocketTextChannel UnbanChannel { get; set; }
    private SocketTextChannel KickChannel { get; set; }
    private SocketTextChannel MuteChannel { get; set; }
    private SocketTextChannel UnmuteChannel { get; set; }
    private SocketTextChannel AdminChatChannel { get; set; }
    private SocketTextChannel ReportChannel { get; set; }
    
    public void Init()
    {
        Instance = this;
    }
    
    public void Unregister()
    {
        BanChannel = null;
        UnbanChannel = null;
        KickChannel = null;
        MuteChannel = null;
        UnmuteChannel = null;
        AdminChatChannel = null;
        ReportChannel = null;
    }
    
    public SocketTextChannel GetBanChannel()
    {
        if(Bot.Handlers.DiscordBot.Instance.Guild == null) return null;
        if(Plugin.Instance.Config.BanChannelId == 0) return null;
        return BanChannel ??= Bot.Handlers.DiscordBot.Instance.Guild.GetTextChannel(Plugin.Instance.Config.BanChannelId);
    }
    
    public SocketTextChannel GetUnbanChannel()
    {
        if(Bot.Handlers.DiscordBot.Instance.Guild == null) return null;
        if(Plugin.Instance.Config.UnbanChannelId == 0) return null;
        return UnbanChannel ??= Bot.Handlers.DiscordBot.Instance.Guild.GetTextChannel(Plugin.Instance.Config.UnbanChannelId);
    }
    
    public SocketTextChannel GetKickChannel()
    {
        if(Bot.Handlers.DiscordBot.Instance.Guild == null) return null;
        if(Plugin.Instance.Config.KickChannelId == 0) return null;
        return KickChannel ??= Bot.Handlers.DiscordBot.Instance.Guild.GetTextChannel(Plugin.Instance.Config.KickChannelId);
    }
    
    public SocketTextChannel GetMuteChannel()
    {
        if(Bot.Handlers.DiscordBot.Instance.Guild == null) return null;
        if(Plugin.Instance.Config.MuteChannelId == 0) return null;
        return MuteChannel ??= Bot.Handlers.DiscordBot.Instance.Guild.GetTextChannel(Plugin.Instance.Config.MuteChannelId);
    }
    
    public SocketTextChannel GetUnmuteChannel()
    {
        if(Bot.Handlers.DiscordBot.Instance.Guild == null) return null;
        if(Plugin.Instance.Config.UnmuteChannelId == 0) return null;
        return UnmuteChannel ??= Bot.Handlers.DiscordBot.Instance.Guild.GetTextChannel(Plugin.Instance.Config.UnmuteChannelId);
    }
    
    public SocketTextChannel GetAdminChatChannel()
    {
        if(Bot.Handlers.DiscordBot.Instance.Guild == null) return null;
        if(Plugin.Instance.Config.AdminChatChannelId == 0) return null;
        return AdminChatChannel ??= Bot.Handlers.DiscordBot.Instance.Guild.GetTextChannel(Plugin.Instance.Config.AdminChatChannelId);
    }
    
    public SocketTextChannel GetReportChannel()
    {
        if(Bot.Handlers.DiscordBot.Instance.Guild == null) return null;
        if(Plugin.Instance.Config.ReportChannelId == 0) return null;
        return ReportChannel ??= Bot.Handlers.DiscordBot.Instance.Guild.GetTextChannel(Plugin.Instance.Config.ReportChannelId);
    }
    
    
    // These 2 functions are here, and public because they are used in DiscordLab.Moderation when the ban commands are used.
    public void SendBanMessage([CanBeNull] string targetName, string targetId, string reason, string issuerName,
        [CanBeNull] string issuerId, string duration)
    {
        SocketTextChannel channel = GetBanChannel();
        if (channel == null)
        {
            if (Plugin.Instance.Config.BanChannelId == 0) return;
            Log.Error("Either the guild is null or the channel is null. So the ban message has failed to send.");
            return;
        }
        EmbedBuilder embed = new();
        embed.WithTitle(Translation.PlayerBanned);
        embed.WithColor(Plugin.GetColor(Plugin.Instance.Config.BanColor));
        if (targetName != null) embed.AddField(Translation.Player, targetName);
        embed.AddField(Translation.PlayerId, targetId);
        embed.AddField(Translation.Reason, reason);
        embed.AddField(Translation.Issuer, issuerName);
        if (issuerId != null) embed.AddField(Translation.IssuerId, issuerId);
        embed.AddField(Translation.Duration, duration);
        channel.SendMessageAsync(embed: embed.Build());
    }

    public void SendUnbanMessage(string targetId)
    {
        SocketTextChannel channel = GetUnbanChannel();
        if (channel == null)
        {
            if (Plugin.Instance.Config.UnbanChannelId == 0) return;
            Log.Error("Either the guild is null or the channel is null. So the unban message has failed to send.");
            return;
        }
        EmbedBuilder embed = new();
        embed.WithTitle(Translation.PlayerUnbanned);
        embed.WithColor(Plugin.GetColor(Plugin.Instance.Config.UnbanColor));
        embed.AddField(Translation.PlayerId, targetId);
        channel.SendMessageAsync(embed: embed.Build());
    }
}