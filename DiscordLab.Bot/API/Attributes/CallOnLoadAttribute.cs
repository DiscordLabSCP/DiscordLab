namespace DiscordLab.Bot.API.Attributes;

using System.Reflection;
using System.Text;
using LabApi.Features.Console;
using NorthwoodLib.Pools;

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
                    LogLoadException(ex, method, type);
                }
            }
        }
    }

    /// <summary>
    /// Logs an exception that is thrown from a method.
    /// </summary>
    /// <param name="ex">The exception that got caught.</param>
    /// <param name="method">The method that the exception was thrown from.</param>
    /// <param name="type">The type the method comes from, isn't required but is useful.</param>
    public static void LogLoadException(Exception ex, MethodInfo method, Type? type = null) =>
        Logger.Error($"Got an exception whilst trying to run {GetFullName(method, type)}:\n{ex}");

    /// <summary>
    /// Gets the full name of a method from it's <see cref="MethodInfo"/> and/or <see cref="Type"/>.
    /// </summary>
    /// <param name="method">The method that you want the name of.</param>
    /// <param name="type">The type that the method is from, isn't required unless dynamic method is called, otherwise just the name of the method will print.</param>
    /// <returns>The full method name.</returns>
    public static string GetFullName(MethodInfo method, Type? type = null)
    {
        StringBuilder builder = StringBuilderPool.Shared.Rent();

        if (method.DeclaringType != null && type != null)
        {
            builder.Append((method.DeclaringType ?? type).FullName);
            builder.Append(':');
        }

        builder.Append(method.Name);

        return StringBuilderPool.Shared.ToStringReturn(builder);
    }
}