using System.Net.WebSockets;
using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Interfaces;
using DiscordLab.Bot.API.Modules;
using Exiled.API.Features;
using UpdateStatus = DiscordLab.Bot.API.Modules.UpdateStatus;

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
                LogLevel = Plugin.Instance.Config.Debug ? LogSeverity.Debug : LogSeverity.Warning
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
                    Log.Error(msg);
                    break;
                case LogSeverity.Warning:
                    Log.Warn(msg);
                    break;
                default:
                    Log.Info(msg);
                    break;
            }

            return Task.CompletedTask;
        }

        private async Task StartClient()
        {
            Log.Debug("Starting Discord bot...");
            await Client.LoginAsync(TokenType.Bot, Plugin.Instance.Config.Token);
            await Client.StartAsync();
        }

        private async Task StopClient()
        {
            await Client.LogoutAsync();
            await Client.StopAsync();
        }

        public SocketGuild GetGuild(ulong id = 0)
        {
            return id == 0 ? _guild : Client.GetGuild(id);
        }

        private async Task Ready()
        {
            _guild = Client.GetGuild(Plugin.Instance.Config.GuildId);
            foreach (ISlashCommand command in SlashCommandLoader.Commands)
            {
                try
                {
                    SocketGuild guild = GetGuild(command.GuildId);
                    if (guild == null)
                    {
                        Log.Warn($"Command {command.Data.Name} failed to register, couldn't find guild {command.GuildId} (from module) nor {Plugin.Instance.Config.GuildId} (from the bot). Make sure your guild IDs are correct.");
                        continue;
                    }
                    await guild.CreateApplicationCommandAsync(command.Data.Build());
                }
                catch (Exception e)
                {
                    Log.Error($"Failed to create guild command '{command.Data.Name}': {e}");
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
            IAutocompleteCommand cmd = (IAutocompleteCommand)commands.FirstOrDefault(c => c.Data.Name == autocomplete.Data.CommandName && c is IAutocompleteCommand);
            if (cmd == null) return;
            await cmd.Autocomplete(autocomplete);
        }
    }
}