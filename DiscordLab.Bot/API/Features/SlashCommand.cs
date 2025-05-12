using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Interfaces;

namespace DiscordLab.Bot.API.Features
{
    public abstract class SlashCommand : IAutocompleteCommand
    {
        public abstract SlashCommandBuilder Data { get; }

        public virtual ulong GuildId { get; set; } = 0;
        
        public abstract Task Run(SocketSlashCommand command);

        public virtual Task Autocomplete(SocketAutocompleteInteraction autocomplete)
        {
            return Task.CompletedTask;
        }
    }
}