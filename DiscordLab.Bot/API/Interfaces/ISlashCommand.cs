namespace DiscordLab.Bot.API.Interfaces
{
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Reflection;
    using Discord;
    using Discord.WebSocket;
    using DiscordLab.Bot.API.Attributes;
    using LabApi.Features.Console;

    /// <summary>
    /// A wrapper to easily add your own slash commands in your bot.
    /// </summary>
    public interface ISlashCommand
    {
        public static ObservableCollection<ISlashCommand> Commands = [];

        /// <summary>
        /// Gets the slash command data.
        /// </summary>
        public SlashCommandBuilder Data { get; }

        /// <summary>
        /// Gets the guild ID to assign this command to.
        /// </summary>
        public ulong GuildId { get; }

        /// <summary>
        /// What should be called when you run this command.
        /// </summary>
        /// <param name="command">The command data.</param>
        /// <returns>A <see cref="Task"/>.</returns>
        public Task Run(SocketSlashCommand command);

        /// <summary>
        /// Finds and creates all slash commands in your plugin. There is no method to delete all your commands, as that is handled by the bot itself.
        /// </summary>
        /// <param name="assembly">The assembly you wish to check, defaults to the current one.</param>
        public static void FindAll(Assembly assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();

            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsAbstract || !typeof(ISlashCommand).IsAssignableFrom(type))
                    continue;

                ISlashCommand init = Activator.CreateInstance(type) as ISlashCommand;
                Commands.Add(init);
            }
        }

        [CallOnLoad]
        private static void Start()
        {
            Commands.CollectionChanged += OnCollectionChanged;
        }

        [CallOnUnload]
        private static void Unload()
        {
            Commands.CollectionChanged -= OnCollectionChanged;
            Commands = null;
        }

        [CallOnReady]
        private static void Ready()
        {
            Task.Run(() => RegisterGuildCommands(Commands));
        }

        private static void OnCollectionChanged(object _, NotifyCollectionChangedEventArgs ev)
        {
            if (!Client.IsClientReady)
                return;
            if (ev.Action is not (NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Replace))
                return;

            Task.Run(() => RegisterGuildCommands((IEnumerable<ISlashCommand>)ev.NewItems));
        }

        private static async Task RegisterGuildCommands(IEnumerable<ISlashCommand> commands)
        {
            foreach (IGrouping<ulong, ISlashCommand> cmds in commands.GroupBy(cmd => cmd.GuildId))
            {
                SocketGuild guild = Client.GetGuild(cmds.Key);
                if (guild == null)
                {
                    Logger.Warn($"Could not find guild {cmds.Key}, so could not register the commands {string.Join(",", cmds.Select(cmd => cmd.Data.Name))}");
                    continue;
                }

                await guild.BulkOverwriteApplicationCommandAsync(cmds.Select(cmd => cmd.Data.Build()).ToArray<ApplicationCommandProperties>());
            }
        }
    }
}