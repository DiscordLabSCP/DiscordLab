namespace DiscordLab.Bot.API.Attributes;

using System.Reflection;
using LabApi.Features.Console;

/// <summary>
/// An attribute that when used on a method, will trigger whenever your plugin is loaded. Requires you to run <see cref="Load"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class CallOnLoadAttribute : Attribute
{
    /// <summary>
    /// Find all <see cref="CallOnLoadAttribute"/> attributes in your plugin and calls them.
    /// </summary>
    /// <param name="assembly">The assembly you wish to check, defaults to the current one.</param>
    public static void Load(Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();

        foreach (Type type in assembly.GetTypes())
        {
            foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public |
                                                          BindingFlags.NonPublic))
            {
                CallOnLoadAttribute attribute = method.GetCustomAttribute<CallOnLoadAttribute>();
                if (attribute == null)
                    continue;

                Logger.Debug($"Loading load attribute {method.Name} from {type.FullName}", Plugin.Instance.Config.Debug);

                try
                {
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