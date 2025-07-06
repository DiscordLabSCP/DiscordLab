namespace DiscordLab.Bot.API.Attributes
{
    using System.Reflection;
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
        public static void Load(Assembly assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();

            foreach (Type type in assembly.GetTypes())
            {
                foreach (MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
                {
                    CallOnReadyAttribute attribute = method.GetCustomAttribute<CallOnReadyAttribute>();
                    if (attribute == null)
                        continue;

                    instances.Add(method);
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
                    method.Invoke(null, null);
                }
                catch (Exception ex)
                {
                    Logger.Error(ex);
                }
            }

            instances = null;
        }
    }
}