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

        public bool IsReady;

        private async Task Ready()
        {
            IsReady = true;
            
            _guild = Client.GetGuild(Plugin.Instance.Config.GuildId);

            // just in case a command gets added whilst the below loop is happening
            List<ISlashCommand> commandsSnapshot = SlashCommandLoader.Commands.ToList();
            
            foreach (ISlashCommand command in commandsSnapshot)
            {
                await CreateGuildCommand(command);
            }
            await Task.CompletedTask;
        }

        public async Task CreateGuildCommand(ISlashCommand command)
        {
            try
            {
                SocketGuild guild = GetGuild(command.GuildId);
                if (guild == null)
                {
                    Log.Warn($"Command {command.Data.Name} failed to register, couldn't find guild {command.GuildId} (from module) nor {Plugin.Instance.Config.GuildId} (from the bot). Make sure your guild IDs are correct.");
                    return;
                }
                await guild.CreateApplicationCommandAsync(command.Data.Build());
            }
            catch (Exception e)
            {
                Log.Error($"Failed to create guild command '{command.Data.Name}': {e}");
            }
        }

        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            ISlashCommand cmd = SlashCommandLoader.Commands.FirstOrDefault(c => c.Data.Name == command.Data.Name);
            if (cmd == null) return;
            await cmd.Run(command);
        }

        private async Task AutoCompleteHandler(SocketAutocompleteInteraction autocomplete)
        {
            IAutocompleteCommand cmd = (IAutocompleteCommand)SlashCommandLoader.Commands.FirstOrDefault(c => c.Data.Name == autocomplete.Data.CommandName && c is IAutocompleteCommand);
            if (cmd == null) return;
            await cmd.Autocomplete(autocomplete);
        }
    }
}