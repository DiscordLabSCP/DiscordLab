using Discord;
using Discord.WebSocket;
using DiscordLab.Bot;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Features;

namespace DiscordLab.Bot.Handlers
{
    public class DiscordBot : IRegisterable
    {
        public static DiscordBot Instance { get; private set; }
        
        public DiscordSocketClient Client { get; private set; }

        public SocketGuild Guild;

        public void Init()
        {
            Instance = this;
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
            Log.Debug("Starting Discord client...");
            DiscordSocketConfig config = new()
            {
                GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages,
                LogLevel = Plugin.Instance.Config.Debug ? LogSeverity.Debug : LogSeverity.Warning
            };
            Client = new(config);
            Client.Log += DiscLog;
            Client.Ready += Ready;
            await Client.LoginAsync(TokenType.Bot, Plugin.Instance.Config.Token);
            await Client.StartAsync();
        }

        private async Task StopClient()
        {
            await Client.LogoutAsync();
            await Client.StopAsync();
        }

        private async Task Ready()
        {
            Guild = Client.GetGuild(Plugin.Instance.Config.GuildId);
            await Task.CompletedTask;
        }
    }
}