﻿using Discord.WebSocket;
using DiscordLab.Bot.API.Extensions;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using Exiled.Events.EventArgs.Server;

namespace DiscordLab.ConnectionLogs.Handlers
{
    public class Events : IRegisterable
    {
        public void Init()
        {
            Exiled.Events.Handlers.Player.Verified += OnPlayerVerified;
            Exiled.Events.Handlers.Player.Left += OnPlayerLeave;
            Exiled.Events.Handlers.Server.RoundStarted += OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded += OnRoundEnded;
        }

        public void Unregister()
        {
            Exiled.Events.Handlers.Player.Verified -= OnPlayerVerified;
            Exiled.Events.Handlers.Player.Left -= OnPlayerLeave;
            Exiled.Events.Handlers.Server.RoundStarted -= OnRoundStarted;
            Exiled.Events.Handlers.Server.RoundEnded -= OnRoundEnded;
        }

        private void OnPlayerVerified(VerifiedEventArgs ev)
        {
            if (Round.InProgress && !string.IsNullOrEmpty(ev.Player.Nickname))
            {
                string message = Plugin.Instance.Translation.PlayerJoin.LowercaseParams().Replace("{player}", ev.Player.Nickname)
                    .Replace("{id}", ev.Player.UserId).PlayerReplace("player", ev.Player).StaticReplace();
                SocketTextChannel channel = DiscordBot.Instance.GetJoinChannel();
                if (channel != null) channel.SendMessageAsync(message);
                else
                    Log.Error(
                        "Either the guild is null or the channel is null. So the join message has failed to send.");
            }
        }

        private void OnPlayerLeave(LeftEventArgs ev)
        {
            if (Round.InProgress && !string.IsNullOrEmpty(ev.Player.Nickname))
            {
                string message = Plugin.Instance.Translation.PlayerLeave.LowercaseParams().Replace("{player}", ev.Player.Nickname)
                    .Replace("{id}", ev.Player.UserId).PlayerReplace("player", ev.Player).StaticReplace();
                SocketTextChannel channel = DiscordBot.Instance.GetLeaveChannel();
                if (channel != null) channel.SendMessageAsync(message);
                else
                    Log.Error(
                        "Either the guild is null or the channel is null. So the leave message has failed to send.");
            }
        }

        private void OnRoundStarted()
        {
            string message = Plugin.Instance.Translation.RoundStart.LowercaseParams();
            SocketTextChannel channel = DiscordBot.Instance.GetRoundStartChannel();
            if (channel == null)
            {
                Log.Error("Either the guild is null or the channel is null. So the round start message has failed to send.");
                return;
            }

            List<Player> playerList = Player.List.Where(p => !p.IsNPC).ToList();
            string players = string.Join("\n", playerList.Select(player => 
                Plugin.Instance.Translation.RoundPlayers.LowercaseParams()
                    .Replace("{playername}", player.Nickname)
                    .Replace("{playerid}", player.UserId)
                    .Replace("{ip}", player.IPAddress)
                    .PlayerReplace("player", player)
                    .StaticReplace()
            ));
            channel.SendMessageAsync(message.Replace("{players}", players).StaticReplace());
        }

        private void OnRoundEnded(RoundEndedEventArgs _)
        {
            string message = Plugin.Instance.Translation.RoundEnd.LowercaseParams();
            if(Plugin.Instance.Config.RoundEndChannelId == 0) return;
            SocketTextChannel channel = DiscordBot.Instance.GetRoundEndChannel();
            if (channel == null)
            {
                Log.Error("Either the guild is null or the channel is null. So the round end message has failed to send.");
                return;
            }

            List<Player> playerList = Player.List.Where(p => !p.IsNPC).ToList();
            string players = string.Join("\n", playerList.Select(player =>
                Plugin.Instance.Translation.RoundPlayers.LowercaseParams()
                    .Replace("{playername}", player.Nickname)
                    .Replace("{playerid}", player.UserId)
                    .Replace("{ip}", player.IPAddress)
                    .PlayerReplace("player", player)
                    .StaticReplace()
            ));
            channel.SendMessageAsync(message.Replace("{players}", players).StaticReplace());
        }
    }
}