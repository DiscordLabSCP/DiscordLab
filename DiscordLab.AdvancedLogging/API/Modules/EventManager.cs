using System.Diagnostics;
using System.Reflection;
using DiscordLab.AdvancedLogging.Handlers;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Interfaces;
using Exiled.Events.Features;
using Exiled.Loader;

namespace DiscordLab.AdvancedLogging.API.Modules
{
    public static class EventManager
    {
        // ReSharper disable once UseCollectionExpression
        // ReSharper disable once MemberCanBePrivate.Global
        public static Type[] HandlerTypes = new Type[0];
        // ReSharper disable once UseCollectionExpression
        private static readonly List<Tuple<PropertyInfo, Delegate>> DynamicHandlers = new ();

        internal static void GetHandlers()
        {
            HandlerTypes = Loader.Plugins.First(plug => plug.Name == "Exiled.Events")
                .Assembly.GetTypes()
                .Where(t => t.FullName?.Equals($"Exiled.Events.Handlers.{t.Name}") is true).ToArray();
        }

        internal static void AddEventHandler(string handler, string @event)
        {
            Type handlerType = HandlerTypes.FirstOrDefault(h => h.Name == handler);
            if (handlerType == null)
            {
                Log.Error($"Handler {handler} not found");
                return;
            }
            
            Delegate @delegate;
            PropertyInfo propertyInfo = handlerType.GetProperty(@event);

            if (propertyInfo == null)
            {
                Log.Error($"Failed to find {@event} under {handler}");
                return;
            }
            
            EventInfo eventInfo = propertyInfo.PropertyType.GetEvent("InnerEvent", (BindingFlags)(-1));
            if (eventInfo == null)
            {
                Log.Error($"Failed to bind {handler}.{@event}");
                return;
            }
            MethodInfo subscribe = propertyInfo.PropertyType.GetMethods().First(x => x.Name is "Subscribe");
            
            if (propertyInfo.PropertyType == typeof(Event))
            {
                @delegate = new CustomEventHandler(EventNoArgs);
            }
            else if (propertyInfo.PropertyType.IsGenericType && propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Event<>))
            {
                @delegate = typeof(EventManager)
                    .GetMethod(nameof(Event))?
                    .MakeGenericMethod(eventInfo.EventHandlerType.GenericTypeArguments)
                    .CreateDelegate(typeof(CustomEventHandler<>)
                        .MakeGenericType(eventInfo.EventHandlerType.GenericTypeArguments));
            }
            else
            {
                Log.Error($"Failed to bind {handler}.{@event}");
                return;
            }

            // ReSharper disable once UseCollectionExpression
            subscribe.Invoke(propertyInfo.GetValue(null), new object[] { @delegate });
            DynamicHandlers.Add(new (propertyInfo, @delegate));
        }

        internal static void RemoveEventHandlers()
        {
            for (int i = 0; i < DynamicHandlers.Count; i++)
            {
                Tuple<PropertyInfo, Delegate> tuple = DynamicHandlers[i];
                PropertyInfo propertyInfo = tuple.Item1;
                Delegate handler = tuple.Item2;

                MethodInfo unSubscribe = propertyInfo.PropertyType.GetMethods().First(x => x.Name is "Unsubscribe");

                // ReSharper disable once CoVariantArrayConversion
                // ReSharper disable once UseCollectionExpression
                unSubscribe.Invoke(propertyInfo.GetValue(null), new[] { handler });
                DynamicHandlers.Remove(tuple);
            }
        }
        
        // ReSharper disable once MemberCanBePrivate.Global
        public static void EventNoArgs()
        {
            StackFrame frame = new(3);
            MethodBase method = frame.GetMethod();
            if (method == null) return;

            string eventName = method.Name.Replace("On", "");
            string handlerName = method.DeclaringType?.Name;
            IEnumerable<Features.Log> logs = DiscordBot.Instance.GetLogs();
            Features.Log log = logs.FirstOrDefault(x => x.Handler == handlerName && x.Event == eventName);
            if (log == null) return;
            
            Log.Debug("Event triggered, routing to " + log.ChannelId);
            
            GenerateEvent.Event(null, DiscordBot.Instance.GetChannel(log.ChannelId), log.Content, Array.Empty<string>());
        }

        public static void Event<T>(T ev) where T : IExiledEvent
        {
            string typePath = typeof(T).FullName;
            string[] parts = typePath!.Split('.');
            string handler = parts[parts.Length - 2];
            string @event = parts[parts.Length - 1].Replace("EventArgs", "");
            IEnumerable<Features.Log> logs = DiscordBot.Instance.GetLogs();
            API.Features.Log log = logs.FirstOrDefault(x => x.Handler == handler && x.Event == @event);
            if (log == null) return;
            
            Log.Debug($"{handler}.{@event} triggered, routing to {log.ChannelId}");

            GenerateEvent.Event(ev, DiscordBot.Instance.GetChannel(log.ChannelId), log.Content, (log.Nullables ?? "").Split(','));
        }
    }
}