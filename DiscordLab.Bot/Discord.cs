using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.Interfaces;
using Exiled.API.Features;
using UnityEngine.PlayerLoop;

namespace DiscordLab.Bot;

public class Discord : IRegisterable
{
    public static Discord Instance;
    
    public static DiscordSocketClient Client;
    
    public static SocketGuild Guild;

    public void Init()
    {
        Instance = this;
        Task.Run(StartClient);
    }

    public void Unregister()
    {
        Task.Run(StopClient);
    }
    
    private static async Task StartClient()
    {
        DiscordSocketConfig config = new()
        {
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages,
            LogLevel = LogSeverity.Error
        };
        Client = new(config);
        Client.Log += DiscordLog;
        Client.Ready += Ready;
        
        await Client.LoginAsync(TokenType.Bot, Plugin.Instance.Config.Token);
        await Client.StartAsync();
    }
    
    private static async Task StopClient()
    {
        await Client.LogoutAsync();
        await Client.StopAsync();
    }
    
    private static Task DiscordLog(LogMessage msg)
    {
        Log.Info(msg);
        return Task.CompletedTask;
    }

    private static Task Ready()
    {
        Guild = Client.GetGuild(Plugin.Instance.Config.GuildId);
        return Task.CompletedTask;
    }
}