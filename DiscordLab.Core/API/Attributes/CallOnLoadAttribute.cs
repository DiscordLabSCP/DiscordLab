using System.Reflection;
using DiscordLab.Core.API.Utilities;
using LabApi.Features.Console;

namespace DiscordLab.Core.API.Attributes;

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

                Logger.Debug($"Invoking {type.FullName}:{method.Name} ({nameof(CallOnLoadAttribute)})", Plugin.Instance.Config.Debug);

                try
                {
                    method.Invoke(null, null);
                }
                catch (Exception ex)
                {
                    LoggingUtils.LogMethodError(ex, method, type);
                }
            }
        }
    }
}