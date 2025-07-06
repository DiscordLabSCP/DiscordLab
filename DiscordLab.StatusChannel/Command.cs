using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Interfaces;

namespace DiscordLab.StatusChannel
{
    public class Command : ISlashCommand
    {
        public SlashCommandBuilder Data { get; } = Plugin.Instance.Translation.PlayerListCommand;

        public ulong GuildId { get; } = Plugin.Instance.Config.GuildId;
        
        public async Task Run(SocketSlashCommand command)
        {
            await command.RespondAsync(embed: Events.GetEmbed().Build());
        }
    }
}