using System.Globalization;
using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Features;
using MEC;
using XPSystem.API;
using XPSystem.API.StorageProviders;
using XPSystem.API.StorageProviders.Models;

namespace DiscordLab.XPSystem.Handlers;

public class DiscordBot : IRegisterable
{
    private static Translation Translation => Plugin.Instance.Translation;
    
    public static DiscordBot Instance { get; private set; }
    
    private SocketTextChannel Channel { get; set; }
    
    public void Init()
    {
        Instance = this;
        Bot.Handlers.DiscordBot.Instance.Client.Ready += Ready;
        Bot.Handlers.DiscordBot.Instance.Client.SlashCommandExecuted += SlashCommandHandler;
    }
    
    public void Unregister()
    {
        Channel = null;
    }

    public SocketTextChannel GetChannel()
    {
        if(Bot.Handlers.DiscordBot.Instance.Guild == null) return null;
        if(Plugin.Instance.Config.ChannelId == 0) return null;
        return Channel ??= Bot.Handlers.DiscordBot.Instance.Guild.GetTextChannel(Plugin.Instance.Config.ChannelId);
    }

    private async Task Ready()
    {
        SlashCommandBuilder command = new();

        command.WithName(Translation.CommandName);
        command.WithDescription(Translation.CommandDescription);
        SlashCommandOptionBuilder option = new();
        option.WithName(Translation.CommandOptionName);
        option.WithDescription(Translation.CommandOptionDescription);
        option.WithType(ApplicationCommandOptionType.String);
        option.IsRequired = true;
        command.AddOption(option);

        Timing.CallDelayed(1f, () =>
        {
            try
            {
                Bot.Handlers.DiscordBot.Instance.Guild.CreateApplicationCommandAsync(command.Build());
            }
            catch (Exception e)
            {
                Log.Error("Failed to create global application command: " + e);
            }
        });

        await Task.CompletedTask;
    }

    private async Task SlashCommandHandler(SocketSlashCommand command)
    {
        if (command.Data.Name != Translation.CommandName) return;
        if (command.Data.Options.Count == 0) return;
        string option = command.Data.Options.First().Value.ToString();
        if (XPAPI.TryParseUserId(option, out IPlayerId<object> playerId) == false)
        {
            await command.RespondAsync(Translation.FailToGetUser);
            return;
        }
        PlayerInfoWrapper info = XPAPI.GetPlayerInfo(playerId);
        EmbedBuilder embed = new()
        {
            Title = Translation.EmbedTitle,
            Description = Translation.EmbedDescription.Replace("{level}", info.Level.ToString()).Replace("{currentxp}", info.XP.ToString()).Replace("{neededxp}", info.NeededXPCurrent.ToString()),
            Footer = new ()
            {
                Text = Translation.EmbedFooter.Replace("{user}", info.Nickname)
                    .Replace("{userid}", option)
            },
            Color = new Color(uint.Parse(Plugin.Instance.Config.Color, NumberStyles.HexNumber))
        };
        await command.RespondAsync(null, [embed.Build()]);
    }
}