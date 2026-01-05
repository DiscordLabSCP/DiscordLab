namespace DiscordLab.Bot.API.Features;

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Reflection;
using Discord;
using Discord.WebSocket;
using DiscordLab.Bot.API.Attributes;
using DiscordLab.Bot.API.Extensions;
using LabApi.Features.Console;

/// <summary>
/// A wrapper to easily add your own slash commands in your bot.
/// </summary>
public abstract class SlashCommand
{
    /// <summary>
    /// The list of currently active <see cref="SlashCommand"/>s.
    /// </summary>
#pragma warning disable SA1401
    public static ObservableCollection<SlashCommand> Commands = [];
#pragma warning restore SA1401

    /// <summary>
    /// Gets the slash command data.
    /// </summary>
    public abstract SlashCommandBuilder Data { get; }

    /// <summary>
    /// Gets the guild ID to assign this command to.
    /// </summary>
    protected abstract ulong GuildId { get; }

    /// <summary>
    /// Finds and creates all slash commands in your plugin. There is no method to delete all your commands, as that is handled by the bot itself.
    /// </summary>
    /// <param name="assembly">The assembly you wish to check, defaults to the current one.</param>
    public static void FindAll(Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();

        foreach (Type type in assembly.GetTypes())
        {
            if (type.IsAbstract || !typeof(SlashCommand).IsAssignableFrom(type))
                continue;

            if (Activator.CreateInstance(type) is not SlashCommand init)
                continue;
            Commands.Add(init);
        }
    }

    /// <summary>
    /// What should be called when you run this command.
    /// </summary>
    /// <param name="command">The command data.</param>
    /// <returns>A <see cref="Task"/>.</returns>
    public abstract Task Run(SocketSlashCommand command);

    [CallOnLoad]
    private static void Start()
    {
        Commands.CollectionChanged += OnCollectionChanged;
    }

    [CallOnUnload]
    private static void Unload()
    {
        Commands.CollectionChanged -= OnCollectionChanged;
        Commands.Clear();
    }

    [CallOnReady]
    private static void Ready()
    {
        Task.RunAndLog(() => RegisterGuildCommands(Commands));
    }

#pragma warning disable SA1313
    private static void OnCollectionChanged(object _, NotifyCollectionChangedEventArgs ev)
#pragma warning restore SA1313
    {
        if (!Client.IsClientReady)
        {
            return;
        }

        if (ev.Action is not (NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Replace))
        {
            return;
        }

        Task.RunAndLog(() => RegisterGuildCommands((IEnumerable<SlashCommand>)ev.NewItems));
    }

    private static async Task RegisterGuildCommands(IEnumerable<SlashCommand> commands)
    {
        foreach (IGrouping<ulong, SlashCommand> cmds in commands.GroupBy(cmd => cmd.GuildId))
        {
            SocketGuild? guild = Client.GetGuild(cmds.Key);
            if (guild == null)
            {
                Logger.Warn(
                    $"Could not find guild {cmds.Key}, so could not register the commands {string.Join(",", cmds.Select(cmd => cmd.Data.Name))}");
                continue;
            }

            await guild.BulkOverwriteApplicationCommandAsync(cmds.Select(cmd => cmd.Data.Build())
                .ToArray<ApplicationCommandProperties>());
        }
    }
}