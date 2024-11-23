using System.Reflection;
using Discord.WebSocket;
using DiscordLab.AdvancedLogging.API.Features;
using DiscordLab.AdvancedLogging.API.Modules;
using DiscordLab.Bot.API.Interfaces;
using DiscordLab.Bot.API.Modules;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs.Interfaces;
using Exiled.Events.Features;
using Newtonsoft.Json.Linq;
using Log = Exiled.API.Features.Log;

namespace DiscordLab.AdvancedLogging.Handlers
{
    public class DiscordBot : IRegisterable
    {
        public static DiscordBot Instance { get; private set; }

        private List<ChannelType> Channels { get; set; }

        private readonly List<Tuple<EventInfo, Delegate>> _dynamicHandlers = new();
        private bool _isHandlerAdded = false;

        public void Init()
        {
            Instance = this;
            Bot.Handlers.DiscordBot.Instance.Client.ModalSubmitted += OnModalSubmitted;
            Bot.Handlers.DiscordBot.Instance.Client.Ready += OnReady;
            Channels = new();
        }

        public void Unregister()
        {
            RemoveEventHandlers();
            Channels = null;
        }

        private void RemoveEventHandlers()
        {
            if (!_isHandlerAdded) return;

            for (int i = 0; i < _dynamicHandlers.Count; i++)
            {
                Tuple<EventInfo, Delegate> tuple = _dynamicHandlers[i];
                EventInfo eventInfo = tuple.Item1;
                Delegate handler = tuple.Item2;

                if (eventInfo.DeclaringType != null)
                {
                    MethodInfo removeMethod = eventInfo.DeclaringType.GetMethod($"remove_{eventInfo.Name}",
                        BindingFlags.Instance | BindingFlags.NonPublic)!;
                    removeMethod.Invoke(null, new object[] { handler });
                }
                else
                {
                    MethodInfo removeMethod = eventInfo.GetRemoveMethod(true);
                    removeMethod.Invoke(null, new object[] { handler });
                }

                _dynamicHandlers.Remove(tuple);
            }

            _isHandlerAdded = false;
        }

        private SocketTextChannel GetChannel(ulong channelId)
        {
            if (Bot.Handlers.DiscordBot.Instance.Guild == null) return null;
            if (Channels.Exists(c => c.ChannelId == channelId))
                return Channels.First(c => c.ChannelId == channelId).Channel;
            SocketTextChannel channel = Bot.Handlers.DiscordBot.Instance.Guild.GetTextChannel(channelId);
            if (channel != null) return channel;
            Log.Error("Either the guild is null or the channel is null. So the status message has failed to send.");
            return null;
        }

        private void GetChannelAndBind(string handler, string @event, ulong channelId)
        {
            SocketTextChannel channel = GetChannel(channelId);
            Channels.Add(new()
            {
                Handler = handler,
                Event = @event,
                Channel = channel,
                ChannelId = channelId
            });
        }

        private async Task OnReady()
        {
            IEnumerable<API.Features.Log> logList = GetLogs();
            foreach (API.Features.Log log in logList)
            {
                GetChannelAndBind(log.Handler, log.Event, log.ChannelId);
                BindEvent(log);
            }

            await Task.CompletedTask;
        }

        public IEnumerable<API.Features.Log> GetLogs()
        {
            JToken logs = WriteableConfig.GetConfig()["AdvancedLogging"];
            if (logs == null)
            {
                WriteableConfig.WriteConfigOption("AdvancedLogging", new JArray());
                return new List<API.Features.Log>();
            }

            return logs.ToObject<IEnumerable<API.Features.Log>>() ?? new List<API.Features.Log>();
        }

        private bool BindEvent(API.Features.Log log)
        {
            IPlugin<IConfig> eventsAssembly =
                Exiled.Loader.Loader.Plugins.FirstOrDefault(x => x.Name == "Exiled.Events");
            if (eventsAssembly == null)
            {
                Log.Error("Could not create any events due to Exiled.Events not being loaded.");
                return false;
            }

            Type eventType = eventsAssembly.Assembly.GetTypes()
                .FirstOrDefault(x => x.Namespace == "Exiled.Events.Handlers" && x.Name == log.Handler);
            if (eventType == null)
            {
                Log.Error($"Handler type 'Exiled.Events.Handlers.{log.Handler}' not found.");
                return false;
            }

            Delegate handler = null;
            PropertyInfo propertyInfo = eventType.GetProperty(log.Event);

            if (propertyInfo == null)
            {
                Log.Error($"Event '{log.Event}' not found in handler '{log.Handler}'.");
                return false;
            }

            EventInfo eventInfo = propertyInfo.PropertyType.GetEvent("InnerEvent", (BindingFlags)(-1))!;

            if (propertyInfo.PropertyType == typeof(Event))
            {
                handler = new CustomEventHandler(() => OnEventTriggeredNoEv(log));

                MethodInfo addMethod = eventInfo.AddMethod.DeclaringType!.GetMethod(
                    $"add_{eventInfo.Name}",
                    BindingFlags.Instance | BindingFlags.NonPublic
                )!;
                addMethod.Invoke(propertyInfo.GetValue(null), new object[] { handler });
            }
            else if (propertyInfo.PropertyType.IsGenericType &&
                     propertyInfo.PropertyType.GetGenericTypeDefinition() == typeof(Event<>))
            {
                MethodInfo eventTriggered = typeof(DiscordBot).GetMethod(nameof(OnEventTriggered),
                    BindingFlags.Instance | BindingFlags.NonPublic);
                MethodInfo genericEventTriggered =
                    eventTriggered!.MakeGenericMethod(eventInfo.EventHandlerType.GenericTypeArguments);
                handler = Delegate.CreateDelegate(eventInfo.EventHandlerType, this, genericEventTriggered);

                MethodInfo addMethod = eventInfo.GetAddMethod(true);
                addMethod.Invoke(propertyInfo.GetValue(null), new object[] { handler });
            }
            else
            {
                Log.Error($"Failed to load event handler for Exiled.Events.Handlers.{log.Handler}.{log.Event}.");
            }

            _dynamicHandlers.Add(new(eventInfo, handler));

            _isHandlerAdded = true;
            return true;
        }

        private void OnEventTriggeredNoEv(API.Features.Log log)
        {
            GenerateEvent.Event(new(), GetChannel(log.ChannelId), log.Content, (log.Nullables ?? "").Split(','));
        }

        private void OnEventTriggered<T>(T ev) where T : IExiledEvent
        {
            string typePath = typeof(T).FullName;
            string[] parts = typePath!.Split('.');
            string handler = parts[parts.Length - 2];
            string @event = parts[parts.Length - 1].Replace("EventArgs", "");
            IEnumerable<API.Features.Log> logs = GetLogs();
            API.Features.Log log = logs.FirstOrDefault(x => x.Handler == handler && x.Event == @event);
            if (log == null) return;

            GenerateEvent.Event(ev, GetChannel(log.ChannelId), log.Content, (log.Nullables ?? "").Split(','));
        }

        private async Task OnModalSubmitted(SocketModal modal)
        {
            if (modal.Data.CustomId != "addlogmodal") return;
            List<SocketMessageComponentData> components = modal.Data.Components.ToList();
            string handler = components.First(x => x.CustomId == "handler").Value;
            string @event = components.First(x => x.CustomId == "event").Value;
            string message = components.First(x => x.CustomId == "message").Value;
            string nullables = components.First(x => x.CustomId == "nullables").Value;
            string channelIdString = components.First(x => x.CustomId == "channel").Value;
            if (!ulong.TryParse(channelIdString, out ulong channelId))
            {
                await modal.RespondAsync("Invalid channel ID", null, false, true);
                return;
            }

            SocketTextChannel channel = GetChannel(channelId);
            if (channel == null)
            {
                await modal.RespondAsync(
                    "Either the guild is null or the channel is null. So I couldn't find the channel you linked.",
                    ephemeral: true);
            }

            Channels.Add(new()
            {
                Handler = handler,
                Event = @event,
                Channel = channel,
                ChannelId = channelId
            });
            JToken logs = WriteableConfig.GetConfig()["AdvancedLogging"];
            if (logs == null)
            {
                WriteableConfig.WriteConfigOption("AdvancedLogging", new JArray());
                logs = WriteableConfig.GetConfig()["AdvancedLogging"]!;
            }

            JArray logList = logs.ToObject<JArray>() ?? new();
            API.Features.Log log = new()
            {
                Handler = handler,
                Event = @event,
                Content = message,
                Nullables = nullables ?? "",
                ChannelId = channelId
            };
            bool eventResponse = BindEvent(log);
            if (eventResponse)
            {
                logList.Add(JObject.FromObject(log));
                WriteableConfig.WriteConfigOption("AdvancedLogging", logList);
                await modal.RespondAsync("Log added", ephemeral: true);
            }
            else
            {
                await modal.RespondAsync("Failed to add log, check server console for more info", ephemeral: true);
            }
        }
    }
}