using DiscordLab.Bot.API.Interfaces;
using DiscordLab.Bot.API.Modules;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;

namespace DiscordLab.BotStatus.Handlers
{
    public class Events : IRegisterable
    {
        public void Init()
        {
            ServerEvents.WaitingForPlayers += OnWaitingForPlayers;
            ServerEvents.RoundStarted += OnRoundStarted;
            PlayerEvents.Joined += OnPlayerVerified;
            PlayerEvents.Left += OnPlayerLeave;
        }

        public void Unregister()
        {
            ServerEvents.WaitingForPlayers -= OnWaitingForPlayers;
            ServerEvents.RoundStarted -= OnRoundStarted;
            PlayerEvents.Joined -= OnPlayerVerified;
            PlayerEvents.Left -= OnPlayerLeave;
        }

        private void OnPlayerVerified(PlayerJoinedEventArgs ev)
        {
            if (Round.IsRoundStarted && !Round.IsRoundEnded) DiscordBot.Instance.SetStatus();
            else
                QueueSystem.QueueRun("DiscordLab.BotStatus.OnPlayerVerified", () =>
                    DiscordBot.Instance.SetStatus()
                );
        }

        private void OnPlayerLeave(PlayerLeftEventArgs ev)
        {
            int players = Player.List.Count(p => p != ev.Player && !p.IsPlayer);
            if ((Round.IsRoundStarted && !Round.IsRoundEnded) || players == 0)
                DiscordBot.Instance.SetStatus(
                    players
                );
            else
                QueueSystem.QueueRun("DiscordLab.BotStatus.OnPlayerLeave", () =>
                    DiscordBot.Instance.SetStatus()
                );
        }

        private void OnRoundStarted()
        {
            DiscordBot.Instance.SetStatus();
        }

        private void OnWaitingForPlayers()
        {
            DiscordBot.Instance.SetStatus();
        }
    }
}