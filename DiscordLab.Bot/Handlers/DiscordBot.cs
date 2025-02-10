﻿using System.Net.WebSockets;
using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Interfaces;
using DiscordLab.Bot.API.Modules;
using LabApi.Features.Console;

namespace DiscordLab.Bot.Handlers
{
    public class DiscordBot : IRegisterable
    {
        public static DiscordBot Instance { get; private set; }
        
        public DiscordSocketClient Client { get; private set; }

        private SocketGuild _guild;

        public void Init()
        {
            Instance = this;
            DiscordSocketConfig config = new()
            {
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages,
                LogLevel = Plugin.Instance.Config!.Debug ? LogSeverity.Debug : LogSeverity.Warning
            };
            Client = new(config);
            Client.Log += DiscLog;
            Client.Ready += Ready;
            Client.SlashCommandExecuted += SlashCommandHandler;
            Client.AutocompleteExecuted += AutoCompleteHandler;
            Task.Run(StartClient);
        }

        public void Unregister()
        {
            Task.Run(StopClient);
        }

        private Task DiscLog(LogMessage msg)
        {
            if (msg.Exception is WebSocketException or GatewayReconnectException)
            {
                return Task.CompletedTask;
            }
            switch (msg.Severity)
            {
                case LogSeverity.Error or LogSeverity.Critical:
                    Logger.Error(msg);
                    break;
                case LogSeverity.Warning:
                    Logger.Warn(msg);
                    break;
                case LogSeverity.Debug:
                    Logger.Debug(msg, Plugin.Instance.Config!.Debug);
                    break;
                default:
                    Logger.Info(msg);
                    break;
            }

            return Task.CompletedTask;
        }

        private async Task StartClient()
        {
            Logger.Debug("Starting Discord bot...", Plugin.Instance.Config!.Debug);
            await Client.LoginAsync(TokenType.Bot, Plugin.Instance.Config!.Token);
            await Client.StartAsync();
        }

        private async Task StopClient()
        {
            await Client.LogoutAsync();
            await Client.StopAsync();
        }

        public SocketGuild GetGuild(ulong id)
        {
            return id == 0 ? _guild : Client.GetGuild(id);
        }

        private async Task Ready()
        {
            _guild = Client.GetGuild(Plugin.Instance.Config!.GuildId);
            foreach (ISlashCommand command in SlashCommandLoader.Commands)
            {
                try
                {
                    SocketGuild guild = GetGuild(command.GuildId);
                    if (guild == null)
                    {
                        Logger.Warn($"Command {command.Data.Name} failed to register, couldn't find guild {command.GuildId} (from module) nor {Plugin.Instance.Config.GuildId} (from the bot). Make sure your guild IDs are correct.");
                        continue;
                    }
                    await guild.CreateApplicationCommandAsync(command.Data.Build());
                }
                catch (Exception e)
                {
                    Logger.Error($"Failed to create guild command '{command.Data.Name}': {e}");
                }
            }
            await Task.CompletedTask;
        }

        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            List<ISlashCommand> commands = SlashCommandLoader.Commands;
            ISlashCommand cmd = commands.FirstOrDefault(c => c.Data.Name == command.Data.Name);
            if (cmd == null) return;
            await cmd.Run(command);
        }
        
        private async Task AutoCompleteHandler(SocketAutocompleteInteraction autocomplete)
        {
            List<ISlashCommand> commands = SlashCommandLoader.Commands;
            ISlashCommand cmd = commands.FirstOrDefault(c => c.Data.Name == autocomplete.Data.CommandName);
            if (cmd == null) return;
            if (cmd.Data.Name == "discordlab")
            {
                if (UpdateStatus.Statuses == null)
                {
                    await autocomplete.RespondAsync(new List<AutocompleteResult>());
                    return;
                }
                await autocomplete.RespondAsync(result: UpdateStatus.Statuses
                    .Where(s => s.ModuleName != "DiscordLab.Bot").Select(s => new AutocompleteResult
                    {
                        Name = s.ModuleName,
                        Value = s.ModuleName
                    }));
            }
        }
    }
}