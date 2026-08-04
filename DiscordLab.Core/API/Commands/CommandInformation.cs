using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using NorthwoodLib.Pools;

namespace DiscordLab.Core.API.Commands;

public struct CommandInformation
{
    public string Name { get; init; }
    
    public IEnumerable<CommandOptionInformation> Options { get; init; }
    
    public Dictionary<string, string>? OptionsDictionary { get; init; }
    
    public CommandInformation(string name, IEnumerable<CommandOptionInformation>? options = null)
    {
        Name = name;
        Options = options ?? [];
        OptionsDictionary = Options.ToDictionary(opt => opt.Name, opt => opt.Value);
    }

    public CommandInformation(string name, IEnumerable<string> optionNames, string message) : this(name, Compile(optionNames, message)) { }

    private static Regex TokenRegex { get; } = new(
        "\"([^\"]*)\"|(\\S+)",
        RegexOptions.Compiled);

    public async Task Reply(MessageInformation info) => await ReplyFunc(info);

    public async Task DeferResponse() => await DeferResponseFunc();

    public Func<MessageInformation, Task> ReplyFunc { private get; init; } = _ => Task.CompletedTask;
    
    public Func<Task> DeferResponseFunc { private get; init; } = () => Task.CompletedTask;
    
    private static IEnumerable<CommandOptionInformation> Compile(IEnumerable<string> optionNames, string message)
    {
        string[] optionNamesArr = optionNames.ToArray();
        
        List<CommandOptionInformation> options = ListPool<CommandOptionInformation>.Shared.Rent();

        MatchCollection matches = TokenRegex.Matches(message);

        int optionNameCount = optionNamesArr.Length;
        
        if (matches.Count < optionNameCount)
            throw new ArgumentException("Not enough inputs");

        for (int i = 0; i < optionNameCount; i++)
        {
            Match m = matches[i];
            string name = optionNamesArr[i];
            string value = m.Groups[1].Success ? m.Groups[1].Value : m.Groups[2].Value;
            
            options.Add(new(name, value));
        }

        ReadOnlyCollection<CommandOptionInformation> returnOptions = new(options);
        
        ListPool<CommandOptionInformation>.Shared.Return(options);

        return returnOptions;
    }
}