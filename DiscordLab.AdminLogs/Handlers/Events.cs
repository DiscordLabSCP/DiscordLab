using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Extensions;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Features;
using LabApi.Events.Handlers;
using LabApi.Features.Console;

namespace DiscordLab.AdminLogs.Handlers
{
    public class Events : IRegisterable
    {
        public void Init()
        {
            ServerEvents.WaitingForPlayers += OnWaitingForPlayers;
        }
        
        public void Unregister()
        {
        }

        private void OnWaitingForPlayers()
        {
            ServerEvents.WaitingForPlayers -= OnWaitingForPlayers;
            if(Plugin.Instance.Config.ServerStartChannelId == 0) return;
            SocketTextChannel channel = DiscordBot.Instance.GetServerStartChannel();
            if (channel == null)
            {
                Logger.Error("Either the guild is null or the channel is null. So the server started message has failed to send.");
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