﻿using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Extensions;
using DiscordLab.Bot.API.Interfaces;
using LabApi.Features.Wrappers;

namespace DiscordLab.Moderation.Commands
{
    public class SendCommand : ISlashCommand
    {
        private static Translation Translation => Plugin.Instance.Translation;

        public SlashCommandBuilder Data { get; } = new()
        {
            Name = Translation.SendCommandName,
            Description = Translation.SendCommandDescription,
            DefaultMemberPermissions = GuildPermission.ManageGuild,
            Options = new()
            {
                new()
                {
                    Name = Translation.SendCommandCommandOptionName,
                    Description = Translation.SendCommandCommandOptionDescription,
                    IsRequired = true,
                    Type = ApplicationCommandOptionType.String
                }
            }
        };
        
        public ulong GuildId { get; set; } = Plugin.Instance.Config.GuildId;
        
        public async Task Run(SocketSlashCommand command)
        {
            await command.DeferAsync(true);
            string commandToExecute = command.Data.Options.First(option => option.Name == Translation.SendCommandCommandOptionName)
                .Value.ToString();

            string response = Server.RunCommand(commandToExecute);
            await command.ModifyOriginalResponseAsync(m => m.Content = Translation.SendCommandResponse.LowercaseParams().Replace("{response}", response));
        }
    }
}