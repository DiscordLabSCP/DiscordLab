using Discord.WebSocket;
using DiscordLab.Bot.API.Extensions;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Map;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;

namespace DiscordLab.RoundLogs.Handlers
{
    public class Events : IRegisterable
    {
        private static Translation Translation => Plugin.Instance.Translation;
        
        public void Init()
        {
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
            Exiled.Events.Handlers.Player.Handcuffing += OnCuffing;
            Exiled.Events.Handlers.Player.RemovedHandcuffs += OnUncuffed;
            Exiled.Events.Handlers.Map.AnnouncingChaosEntrance += OnChaosEnter;
            Exiled.Events.Handlers.Map.AnnouncingNtfEntrance += OnNtfEnter;
        }

        public void Unregister()
        {
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;
            Exiled.Events.Handlers.Player.Handcuffing -= OnCuffing;
            Exiled.Events.Handlers.Player.RemovedHandcuffs -= OnUncuffed;
            Exiled.Events.Handlers.Map.AnnouncingChaosEntrance -= OnChaosEnter;
            Exiled.Events.Handlers.Map.AnnouncingNtfEntrance -= OnNtfEnter;
        }
        
        private void OnRoundStarted()
        {
            SocketTextChannel channel = DiscordBot.Instance.GetRoundStartChannel();
            if (channel == null)
            {
                if(Plugin.Instance.Config.RoundStartChannelId == 0) 
                    return;
                Log.Error("Either the guild is null or the channel is null. So the round start message has failed to send.");
                return;
            }

            channel.SendMessageAsync(Translation.RoundStartMessage.LowercaseParams().StaticReplace());
        }
        
        private void OnRoundEnded(RoundEndedEventArgs ev)
        {
            SocketTextChannel channel = DiscordBot.Instance.GetRoundEndChannel();
            if (channel == null)
            {
                if(Plugin.Instance.Config.RoundEndChannelId == 0) 
                    return;
                Log.Error("Either the guild is null or the channel is null. So the round end message has failed to send.");
                return;
            }

            channel.SendMessageAsync(Translation.RoundEndMessage.LowercaseParams().Replace("{team}", ev.LeadingTeam.ToString()).StaticReplace());
        }
        
        private void OnCuffing(HandcuffingEventArgs ev)
        {
            SocketTextChannel channel = DiscordBot.Instance.GetCuffedChannel();
            if (channel == null)
            {
                if(Plugin.Instance.Config.CuffedChannelId == 0) 
                    return;
                Log.Error("Either the guild is null or the channel is null. So the cuffed message has failed to send.");
                return;
            }

            channel.SendMessageAsync(Translation.PlayerCuffedMessage.LowercaseParams().PlayerReplace("player", ev.Target).PlayerReplace("issuer", ev.Player).StaticReplace());
        }

        private void OnUncuffed(RemovedHandcuffsEventArgs ev)
        {
            SocketTextChannel channel = DiscordBot.Instance.GetUncuffedChannel();
            if (channel == null)
            {
                if(Plugin.Instance.Config.UncuffedChannelId == 0) 
                    return;
                Log.Error("Either the guild is null or the channel is null. So the uncuffed message has failed to send.");
                return;
            }

            channel.SendMessageAsync(Translation.PlayerUncuffedMessage.LowercaseParams().PlayerReplace("player", ev.Target).PlayerReplace("issuer", ev.Player).StaticReplace());
        }

        private void OnNtfEnter(AnnouncingNtfEntranceEventArgs ev)
        {
            SocketTextChannel channel = DiscordBot.Instance.GetNtfEnterChannel();
            if (channel == null)
            {
                if(Plugin.Instance.Config.NtfSpawnChannelId == 0) 
                    return;
                Log.Error("Either the guild is null or the channel is null. So the ntf enter message has failed to send.");
                return;
            }
            
            channel.SendMessageAsync(Translation.NtfSpawnMessage.LowercaseParams().Replace("{unitname}", ev.UnitName).Replace("{unitnumber}", ev.UnitNumber.ToString()).StaticReplace());
        }
        
        private void OnChaosEnter(AnnouncingChaosEntranceEventArgs ev)
        {
            SocketTextChannel channel = DiscordBot.Instance.GetChaosEnterChannel();
            if (channel == null)
            {
                if(Plugin.Instance.Config.ChaosSpawnChannelId == 0) 
                    return;
                Log.Error("Either the guild is null or the channel is null. So the chaos enter message has failed to send.");
                return;
            }
            
            channel.SendMessageAsync(Translation.ChaosSpawnMessage.LowercaseParams().StaticReplace());
        }
    }
}