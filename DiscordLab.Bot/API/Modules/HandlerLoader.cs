using System.Reflection;
using DiscordLab.Bot.API.Interfaces;

namespace DiscordLab.Bot.API.Modules;

public class HandlerLoader
{
    private readonly List<IRegisterable> _inits = new();
    
    public void Load(Assembly assembly)
    {
        Type registerType = typeof(IRegisterable);
        foreach (Type type in assembly.GetTypes())
        {
            if (type.IsAbstract || !registerType.IsAssignableFrom(type))
                continue;

            IRegisterable init = Activator.CreateInstance(type) as IRegisterable;
            _inits.Add(init);
            init!.Init();
        }

        SlashCommandLoader.LoadCommands(assembly);
    }

    public void Unload()
    {
        foreach (IRegisterable init in _inits)
            init.Unregister();
    }
}