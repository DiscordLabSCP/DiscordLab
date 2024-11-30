using Discord.WebSocket;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Extensions;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Player;

namespace DiscordLab.SCPSwap.Handlers
{
    public class Events : IRegisterable
    {
        public void Init()
        {
            Exiled.Events.Handlers.Player.ChangingRole += OnChangingRole;
        }
        
        public void Unregister()
        {
            Exiled.Events.Handlers.Player.ChangingRole -= OnChangingRole;
        }

        private void OnChangingRole(ChangingRoleEventArgs ev)
        {
            if (!ev.Player.IsScp) return;
            if (!ev.NewRole.IsScp()) return;
            SocketTextChannel channel = DiscordBot.Instance.GetChannel();
            if (channel == null)
            {
                Log.Error("Either the guild is null or the channel is null. So the SCP log message has failed to send.");
                return;
            }

            channel.SendMessageAsync(Plugin.Instance.Translation.Message
                .Replace("{player}", ev.Player.Nickname)
                .Replace("{playerid}", ev.Player.UserId)
                .Replace("{oldrole}", ev.Player.Role.Name)
                .Replace("{newrole}", ev.NewRole.GetFullName())
            );
        }
    }
}