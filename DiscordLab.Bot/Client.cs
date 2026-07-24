// ReSharper disable MemberCanBePrivate.Global

using DiscordLab.Core.API.Commands;

namespace DiscordLab.Bot;

using System.Net;
using System.Net.WebSockets;
using System.Reflection;
using System.Text;
using Discord;
using Discord.Net;
using Discord.Net.Rest;
using Discord.Net.WebSockets;
using Discord.WebSocket;
using DiscordLab.Core.API.Attributes;
using DiscordLab.Core.API.Extensions;
using LabApi.Features.Console;
using NorthwoodLib.Pools;

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
        if (id == 0)
            return null;

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
            GatewayIntents = GatewayIntents.All,
            LogLevel = Config.Debug ? LogSeverity.Debug : LogSeverity.Warning,
            RestClientProvider = DefaultRestClientProvider.Create(),
            WebSocketProvider = DefaultWebSocketProvider.Create(),
            MessageCacheSize = Config.MessageCacheSize,
        };

        DebugLog("Done the initial setup...");

        try
        {
            SocketClient = new(config);

            DebugLog("Client has been created...");

            SocketClient.Log += OnLog;
            SocketClient.Ready += OnReady;
            SocketClient.SlashCommandExecuted += OnCommand;
            SocketClient.AutocompleteExecuted += OnAutocomplete;
        }
        catch (TargetInvocationException ex) when (ex.InnerException is TypeLoadException)
        {
            StringBuilder builder = StringBuilderPool.Shared.Rent();
            builder.AppendLine("You may have setup DiscordLab incorrectly, or used another Discord bot in the past.");
            builder.AppendLine(
                "Please ensure you have no conflicting dependencies. This can either be triggered by duplication of the Discord dependencies, or Newtonsoft.Json.");
            builder.AppendLine(
                "Some plugins might implement either of the 2 listed dependencies above, so if you have no duplications at all, you will manually need to remove plugins to see the culprit.");
            builder.AppendLine(
                "If you find a plugin that doesn't work with DiscordLab, please join our Discord and report it there with a link to the repository. We can not fix private plugins.");
            builder.AppendLine("Discord link: https://discord.gg/XBzuGbsNZK");
            Logger.Error(StringBuilderPool.Shared.ToStringReturn(builder));
            throw;
        }

        DebugLog("Client events subscribed...");

        Task.RunAndLog(StartClient);
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
        SocketClient.SlashCommandExecuted -= OnCommand;
        SocketClient.AutocompleteExecuted -= OnAutocomplete;
        Task.RunAndLog(async () =>
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
        switch (msg.Exception)
        {
            case WebSocketException { InnerException: WebSocketClosedException { CloseCode: 4014 } }:
                Logger.Error("DiscordLab requires that you have all Privileged Gateway Intents enabled, you can do this in the \"Bot\" panel of your application. Restart your server when this is complete.");
                return Task.CompletedTask;
            case WebSocketException or GatewayReconnectException when !Config.Debug:
                return Task.CompletedTask;
            default:
                switch (msg.Severity)
                {
                    case LogSeverity.Error or LogSeverity.Critical:
                        Logger.Error(msg);
                        break;
                    case LogSeverity.Warning:
                        if (msg.Source.ToLower().Trim() == "gateway" && msg.Message.Contains("consider removing"))
                            break;

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
    }

    private static Task OnReady()
    {
        DebugLog("Bot is ready");
        IsClientReady = true;
        DefaultGuild = SocketClient.GetGuild(Config.GuildId);

        if (Config.Debug)
        {
            DebugLog(string.Join("\n", SocketClient.Guilds.Select(GenerateGuildChannelsMessage)));
        }

        return Task.CompletedTask;
    }

    private static IEnumerable<CommandOptionInformation> LoopThroughOptions(IEnumerable<SocketSlashCommandDataOption> options) => 
        options.Select(option => new CommandOptionInformation(option.Name, option.Value.ToString(), option.Options.Any() ? LoopThroughOptions(option.Options) : null));

    private static async Task OnCommand(SocketSlashCommand cmd)
    {
        IEnumerable<CommandOptionInformation> options = LoopThroughOptions(cmd.Data.Options);

        CommandInformation information = new(cmd.CommandName, options)
        {
            ReplyFunc = async info =>
            {
                if (!cmd.HasResponded)
                {
                    FileAttachment? attachment = MessageHandler.ToAttachment(info);
                    Embed[]? embeds = MessageHandler.ToEmbeds(info);
                    if (attachment.HasValue)
                        await cmd.RespondWithFileAsync(attachment.Value, info.Content, embeds);
                    else
                        await cmd.RespondAsync(info.Content, embeds);
                }
                else
                {
                    await cmd.ModifyOriginalResponseAsync(msg => MessageHandler.SetFrom(msg, info));
                }
            },
            DeferResponseFunc = async () => await cmd.DeferAsync()
        };

        await CommandHandler.Instance.ExecuteCommand(information);
    }

    private static async Task OnAutocomplete(SocketAutocompleteInteraction autocomplete)
    {
        CommandOptionInformation info = new()
        {
            Name = autocomplete.Data.Current.Name,
            Value = autocomplete.Data.Current.Value.ToString()
        };

        IEnumerable<(string name, string value)> output = CommandHandler.Instance.ExecuteAutocomplete(autocomplete.Data.CommandName, info).Take(25);

        IEnumerable<AutocompleteResult> results = output.Select(tuple => new AutocompleteResult(tuple.name, tuple.value));

        await autocomplete.RespondAsync(results);
    }

    private static string GenerateGuildChannelsMessage(SocketGuild guild) =>
        $"Guild {guild.Name} ({guild.Id}) channels: {string.Join("\n", guild.Channels.Where(channel => channel is SocketTextChannel).Select(GenerateChannelMessage))}";

    private static string GenerateChannelMessage(SocketGuildChannel channel) => $"{channel.Name} ({channel.Id})";

    private static void DebugLog(object message)
    {
        Logger.Debug(message, Config.Debug);
    }
}