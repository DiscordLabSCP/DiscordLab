namespace DiscordLab.Bot.API.Attributes;

using System.Reflection;
using LabApi.Features.Console;

/// <summary>
/// An attribute that when used on a method, will trigger whenever your plugin is unloaded. Requires you to run <see cref="Unload"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class CallOnUnloadAttribute : Attribute
{
    /// <summary>
    /// Find all <see cref="CallOnUnloadAttribute"/> attributes in your plugin and calls them.
    /// </summary>
    /// <param name="assembly">The assembly you wish to check, defaults to the current one.</param>
    public static void Unload(Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();

        foreach (Type type in assembly.GetTypes())
        {
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
            {
                CallOnUnloadAttribute attribute = method.GetCustomAttribute<CallOnUnloadAttribute>();
                if (attribute == null)
                    continue;

                try
                {
                    Logger.Debug($"Calling unload attribute {method.Name} from {type.FullName}", Plugin.Instance.Config.Debug);
                    method.Invoke(null, null);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }
        }
    }
}