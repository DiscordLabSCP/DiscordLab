﻿using System.Globalization;
using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Extensions;
using DiscordLab.Bot.API.Interfaces;
using XPSystem.API;
using XPSystem.API.StorageProviders;
using XPSystem.API.StorageProviders.Models;

namespace DiscordLab.XPSystem.Commands
{
    public class GetLevel : ISlashCommand
    {
        public SlashCommandBuilder Data { get; } = new()
        {
            Name = Plugin.Instance.Translation.CommandName,
            Description = Plugin.Instance.Translation.CommandDescription,
            Options = new()
            {
                new()
                {
                    Name = Plugin.Instance.Translation.CommandOptionName,
                    Description = Plugin.Instance.Translation.CommandOptionDescription,
                    Type = ApplicationCommandOptionType.String,
                    IsRequired = true
                }
            }
        };
        
        public ulong GuildId { get; set; } = Plugin.Instance.Config.GuildId;

        public async Task Run(SocketSlashCommand command)
        {
            if (command.Data.Options.Count == 0) return;
            string option = command.Data.Options.First().Value.ToString();
            if (XPAPI.TryParseUserId(option, out IPlayerId playerId) == false)
            {
                await command.RespondAsync(Plugin.Instance.Translation.FailToGetUser);
                return;
            }

            PlayerInfoWrapper info = XPAPI.GetPlayerInfo(playerId);
            EmbedBuilder embed = new()
            {
                Title = Plugin.Instance.Translation.EmbedTitle,
                Description = Plugin.Instance.Translation.EmbedDescription.LowercaseParams().Replace("{level}", info.Level.ToString())
                    .Replace("{currentxp}", info.XP.ToString()).Replace("{neededxp}", info.NeededXPCurrent.ToString()).StaticReplace(),
                Footer = new()
                {
                    Text = Plugin.Instance.Translation.EmbedFooter.LowercaseParams().Replace("{user}", info.Nickname)
                        .Replace("{userid}", option).StaticReplace()
                },
                Color = new Color(uint.Parse(Plugin.Instance.Config.Color, NumberStyles.HexNumber))
            };
            await command.RespondAsync(embed:embed.Build());
        }
    }
}