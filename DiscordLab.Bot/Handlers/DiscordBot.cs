using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Interfaces;
using DiscordLab.Bot.API.Modules;
using Exiled.API.Features;

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
            if(msg.Severity == LogSeverity.Error || msg.Severity == LogSeverity.Critical) Log.Error(msg);
            else Log.Info(msg);
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

        public SocketGuild GetGuild(ulong id)
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