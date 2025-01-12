using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Extensions;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Features;

namespace DiscordLab.AdminLogs.Handlers
{
    public class Events : IRegisterable
    {
        public void Init()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers += OnWaitingForPlayers;
        }
        
        public void Unregister()
        {
        }

        private void OnWaitingForPlayers()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= OnWaitingForPlayers;
            if(Plugin.Instance.Config.ServerStartChannelId == 0) return;
            SocketTextChannel channel = DiscordBot.Instance.GetServerStartChannel();
            if (channel == null)
            {
                Log.Error("Either the guild is null or the channel is null. So the server started message has failed to send.");
                return;
            }

            EmbedBuilder embed = new()
            {
                Title = Plugin.Instance.Translation.ServerStart,
            };

            if (Plugin.Instance.Translation.ServerStartDescription != null)
            {
                embed.Description = Plugin.Instance.Translation.ServerStartDescription.LowercaseParams().StaticReplace();
            }
            
            channel.SendMessageAsync(embed: embed.Build());
        }
    }
}