using System.ComponentModel;
using System.Reflection;
using DiscordLab.Bot.API.Interfaces;
using DiscordLab.Bot.Handlers;
using Exiled.API.Features;

namespace DiscordLab.Bot.API.Modules
{
    public static class SlashCommandLoader
    {
        internal static void Create()
        {
            Commands.AddingNew += OnCommandAdded;
        }
        
        public static BindingList<ISlashCommand> Commands = new();
        
        /// <summary>
        /// Adds all commands in a <see cref="Assembly"/> from the <see cref="ISlashCommand"/> classes to a list.
        /// </summary>
        /// <param name="assembly">
        /// Your plugin's <see cref="Assembly"/>.
        /// </param>
        public static void LoadCommands(Assembly assembly)
        {
            Type registerType = typeof(ISlashCommand);
            foreach (Type type in assembly.GetTypes())
            {
                if (type.IsAbstract || !registerType.IsAssignableFrom(type))
                    continue;

                ISlashCommand init = Activator.CreateInstance(type) as ISlashCommand;
                Commands.Add(init);
            }
        }

        /// <summary>
        /// This clears all commands from the list.
        /// </summary>
        /// <remarks>
        /// This should only be used by the main bot, you should have no reason to run this.
        /// </remarks>
        public static void ClearCommands()
        {
            Commands = new();
        }

        internal static void Destroy()
        {
            Commands.AddingNew -= OnCommandAdded;
            ClearCommands();
        }

        private static void OnCommandAdded(object sender, AddingNewEventArgs ev)
        {
            ISlashCommand command = (ISlashCommand)ev.NewObject;
            Log.Debug($"Added command {command.Data.Name}, processing...");
            if (!DiscordBot.Instance.IsReady) return;
            Task.Run(() => DiscordBot.Instance.CreateGuildCommand(command));
        }
    }
}