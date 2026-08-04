using System.Collections.ObjectModel;
using System.Reflection;

namespace DiscordLab.Core.API.Commands;

public interface ICommand
{
    public CommandBuilder Data { get; }

    public bool ShouldRegister { get; }

    public Task Execute(CommandInformation data);

    public static ObservableCollection<ICommand> Commands = new();
    
    /// <summary>
    /// Finds and creates all slash commands in your plugin. There is no method to delete all your commands, as that is handled by the bot itself.
    /// </summary>
    /// <param name="assembly">The assembly you wish to check, defaults to the current one.</param>
    public static void FindAll(Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();

        foreach (Type type in assembly.GetTypes())
        {
            if (type.IsAbstract || !typeof(ICommand).IsAssignableFrom(type))
                continue;

            if (Activator.CreateInstance(type) is not ICommand init)
                continue;
            
            if(init.ShouldRegister)
                Commands.Add(init);
        }
    }
}