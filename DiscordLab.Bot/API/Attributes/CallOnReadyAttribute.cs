namespace DiscordLab.Bot.API.Attributes;

using System.Reflection;
using DiscordLab.Bot.API.Utilities;
using LabApi.Features.Console;

/// <summary>
/// An attribute that when used on a method, will trigger whenever the <see cref="Client"/> is ready.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public class CallOnReadyAttribute : Attribute
{
    private static List<MethodInfo> instances = [];

    /// <summary>
    /// Locates all <see cref="CallOnReadyAttribute"/>'s in your plugin and prepares them to be called.
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
                CallOnReadyAttribute attribute = method.GetCustomAttribute<CallOnReadyAttribute>();
                if (attribute == null)
                    continue;

                Logger.Debug($"Loading {type.FullName}:{method.Name} ({nameof(CallOnReadyAttribute)})", Plugin.Instance.Config.Debug);

                // Sometimes the client is ready, so we check here to make sure that it isn't, and if it is just run the method directly.
                if (!Client.IsClientReady)
                {
                    instances.Add(method);
                }
                else
                {
                    LogInvoke(method);
                    method.Invoke(null, null);
                }
            }
        }
    }

    /// <summary>
    /// Called whenever the bot is ready.
    /// </summary>
    internal static void Ready()
    {
        foreach (MethodInfo method in instances)
        {
            try
            {
                LogInvoke(method);
                method.Invoke(null, null);
            }
            catch (Exception ex)
            {
                LoggingUtils.LogMethodError(ex, method);
            }
        }

        instances.Clear();
    }

    private static void LogInvoke(MethodInfo method) => Logger.Debug($"Invoking {LoggingUtils.GetFullName(method)} ({nameof(CallOnReadyAttribute)})", Plugin.Instance.Config.Debug);
}