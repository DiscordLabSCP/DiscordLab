using System.Collections.Specialized;
using DiscordLab.Core.API.Commands;
using DiscordLab.Core.API.Extensions;

namespace DiscordLab.Core;

public abstract class CommandHandler
{
    protected CommandHandler()
    {
        ICommand.Commands.CollectionChanged += CollectionChanged;
    }

    ~CommandHandler()
    {
        ICommand.Commands.CollectionChanged -= CollectionChanged;
    }

    private void CollectionChanged(object _, NotifyCollectionChangedEventArgs ev)
    {
        if (ev.Action is not (NotifyCollectionChangedAction.Add or NotifyCollectionChangedAction.Replace))
            return;

        Task.RunAndLog(() => AddCommands((IEnumerable<ICommand>)ev.NewItems));
    }
    
    public async Task ExecuteCommand(CommandInformation information)
    {
        ICommand? command = ICommand.Commands.FirstOrDefault(cmd => cmd.Data.Name == information.Name);
        if (command == null)
            return;

        await command.Execute(information);
    }

    private CommandOptionBuilder? FindBuilder(CommandOptionBuilder builder, string name)
    {
        if (builder.Name == name)
            return builder;

        if (builder.Options == null)
            return null;
        
        CommandOptionBuilder? retOpt = null;
        
        foreach (CommandOptionBuilder commandOptionBuilder in builder.Options)
        {
            retOpt = FindBuilder(commandOptionBuilder, name);
            if (retOpt != null)
                break;
        }

        return retOpt;
    }

    public IEnumerable<(string name, string value)> ExecuteAutocomplete(string commandName, CommandOptionInformation information)
    {
        ICommand? command = ICommand.Commands.FirstOrDefault(cmd => cmd.Data.Name == information.Name);
        if (command == null)
            return [];

        CommandOptionBuilder? builder = null;
        
        foreach (CommandOptionBuilder commandOptionBuilder in command.Data.Options ?? [])
        {
            builder = FindBuilder(commandOptionBuilder, information.Name);
            if (builder != null)
                break;
        }

        if (builder == null)
            return [];

        IEnumerable<(string name, string value)> output = builder.Autocomplete!(information);

        return output;
    }

    protected abstract Task AddCommands(IEnumerable<ICommand> commands);
}