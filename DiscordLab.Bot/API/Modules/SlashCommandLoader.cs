using System.Reflection;
using Discord;
using DiscordLab.Bot.API.Interfaces;
using DiscordLab.Bot.Handlers;

namespace DiscordLab.Bot.API.Modules
{
    public static class SlashCommandLoader
    {
        public static List<ISlashCommand> Commands = new();

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

        public static void ClearCommands()
        {
            Commands = new();
        }
    }
}