// ReSharper disable MemberCanBePrivate.Global

namespace DiscordLab.Bot;

using System.Net;
using System.Net.WebSockets;
using Discord;
using Discord.Net.Rest;
using Discord.Net.WebSockets;
using Discord.WebSocket;
using DiscordLab.Bot.API.Attributes;
using DiscordLab.Bot.API.Features;
using LabApi.Features.Console;

/// <summary>
/// The Discord bot client.
/// </summary>
public static class Client
{
    /// <summary>
    /// Gets the websocket client for the Discord bot.
    /// </summary>
    public static DiscordSocketClient SocketClient { get; private set; } = null!;

    /// <summary>
    /// Gets a value indicating whether the client is in the ready state.
    /// </summary>
    public static bool IsClientReady { get; private set; }

    /// <summary>
    /// Gets a list of saved text channels listed by their ID.
    /// </summary>
    public static Dictionary<ulong, SocketTextChannel> SavedTextChannels { get; private set; } = new();

    /// <summary>
    /// Gets the default guild for the plugin.
    /// </summary>
    public static SocketGuild? DefaultGuild { get; private set; }

    private static Config Config => Plugin.Instance.Config;

    /// <summary>
    /// Gets a cached guild from a <see cref="ulong" /> ID.
    /// </summary>
    /// <param name="id">The guild ID.</param>
    /// <returns>If the ID is 0, then the default guild (if it exists), if else then it will return the found guild, or null.</returns>
    public static SocketGuild? GetGuild(ulong id)
    {
        return id == 0 ? DefaultGuild : SocketClient.GetGuild(id);
    }

    /// <summary>
    /// Gets or adds a channel via its ID. Uses cache.
    /// </summary>
    /// <param name="id">The ID of the channel.</param>
    /// <returns>The channel, if found.</returns>
    public static SocketTextChannel? GetOrAddChannel(ulong id)
    {
        if (SavedTextChannels.TryGetValue(id, out SocketTextChannel ret))
            return ret;

        SocketChannel channel = SocketClient.GetChannel(id);
        if (channel is not SocketTextChannel text)
            return null;

        SavedTextChannels.Add(id, text);
        return text;
    }

#nullable disable
    /// <summary>
    /// Tries to get or add a channel via its ID. Uses cache.
    /// </summary>
    /// <param name="id">The ID of the channel.</param>
    /// <param name="channel">The channel, if found.</param>
    /// <returns>Whether the channel was found.</returns>
    public static bool TryGetOrAddChannel(ulong id, out SocketTextChannel channel)
    {
        channel = GetOrAddChannel(id);

        return channel != null;
    }
#nullable restore

    /// <summary>
    /// Starts the bot.
    /// </summary>
    [CallOnLoad]
    internal static void Start()
    {
        DebugLog("Starting the Client");
        DiscordSocketConfig config = new()
        {
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMessages,
            LogLevel = Config.Debug ? LogSeverity.Debug : LogSeverity.Warning,
            RestClientProvider = DefaultRestClientProvider.Create(),
            WebSocketProvider = DefaultWebSocketProvider.Create(),
        };

        if (!string.IsNullOrEmpty(Config.ProxyUrl))
        {
            DebugLog("Proxy is configured.");
            WebProxy proxy = new(Config.ProxyUrl);
            config.RestClientProvider = DefaultRestClientProvider.Create(true, proxy);
            config.WebSocketProvider = DefaultWebSocketProvider.Create(proxy);
        }

        DebugLog("Done the initial setup...");

        SocketClient = new(config);

        DebugLog("Client has been created...");

        SocketClient.Log += OnLog;
        SocketClient.Ready += OnReady;
        SocketClient.SlashCommandExecuted += SlashCommandHandler;
        SocketClient.AutocompleteExecuted += AutocompleteHandler;

        DebugLog("Client events subscribed...");

        Task.Run(StartClient);
    }

    /// <summary>
    /// Disables the bot.
    /// </summary>
    [CallOnUnload]
    internal static void Disable()
    {
        SavedTextChannels.Clear();

        SocketClient.Log -= OnLog;
        SocketClient.Ready -= OnReady;
        SocketClient.SlashCommandExecuted -= SlashCommandHandler;
        SocketClient.AutocompleteExecuted -= AutocompleteHandler;
        Task.Run(async () =>
        {
            await SocketClient.LogoutAsync();
            await SocketClient.StopAsync();
            await SocketClient.DisposeAsync();
        });
    }

    private static async Task StartClient()
    {
        DebugLog("Starting client...");
        await SocketClient.LoginAsync(TokenType.Bot, Config.Token);
        await SocketClient.StartAsync();
    }

    private static Task OnLog(LogMessage msg)
    {
        if (msg.Exception is WebSocketException or GatewayReconnectException)
            return Task.CompletedTask;

        switch (msg.Severity)
        {
            case LogSeverity.Error or LogSeverity.Critical:
                Logger.Error(msg);
                break;
            case LogSeverity.Warning:
                Logger.Warn(msg);
                break;
            case LogSeverity.Debug:
                DebugLog(msg);
                break;
            default:
                Logger.Info(msg);
                break;
        }

        return Task.CompletedTask;
    }

    private static Task OnReady()
    {
        DebugLog("Bot is ready");
        IsClientReady = true;
        DefaultGuild = SocketClient.GetGuild(Config.GuildId);
        CallOnReadyAttribute.Ready();
        return Task.CompletedTask;
    }

    private static Task SlashCommandHandler(SocketSlashCommand command)
    {
        DebugLog($"{command.Data.Name} requested a response, finding the command...");
        SlashCommand? cmd = SlashCommand.Commands.FirstOrDefault(c => c.Data.Name == command.Data.Name);

        cmd?.Run(command);
        return Task.CompletedTask;
    }

    private static Task AutocompleteHandler(SocketAutocompleteInteraction autocomplete)
    {
        DebugLog($"{autocomplete.Data.CommandName} requested a response, finding the command...");
        AutocompleteCommand? command =
            SlashCommand.Commands.FirstOrDefault(c =>
                c is AutocompleteCommand cmd && cmd.Data.Name == autocomplete.Data.CommandName) as AutocompleteCommand;

        command?.Autocomplete(autocomplete);
        return Task.CompletedTask;
    }

    private static void DebugLog(object message)
    {
        Logger.Debug(message, Config.Debug);
    }
}