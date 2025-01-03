using System.Diagnostics;
using System.Reflection;
using Discord.WebSocket;
using DiscordLab.AdvancedLogging.API.Features;
using DiscordLab.AdvancedLogging.API.Modules;
using DiscordLab.Bot.API.Interfaces;
using DiscordLab.Bot.API.Modules;
using Exiled.API.Interfaces;
using Exiled.Events.EventArgs.Interfaces;
using Exiled.Events.Features;
using MEC;
using Newtonsoft.Json.Linq;
using Utf8Json.Internal;
using Log = Exiled.API.Features.Log;

namespace DiscordLab.AdvancedLogging.Handlers
{
    public class DiscordBot : IRegisterable
    {
        public static DiscordBot Instance { get; private set; }

        private List<ChannelType> Channels { get; set; }

        public void Init()
        {
            Instance = this;
            Channels = new();
            Bot.Handlers.DiscordBot.Instance.Client.ModalSubmitted += OnModalSubmitted;
            Bot.Handlers.DiscordBot.Instance.Client.Ready += OnReady;
        }

        public void Unregister()
        {
            Channels = null;
        }

        internal SocketTextChannel GetChannel(ulong channelId)
        {
            SocketGuild guild = Bot.Handlers.DiscordBot.Instance.GetGuild(Plugin.Instance.Config.GuildId);
            if (guild == null) return null;
            if (Channels.Exists(c => c.ChannelId == channelId))
                return Channels.First(c => c.ChannelId == channelId).Channel;
            SocketTextChannel channel = guild.GetTextChannel(channelId);
            if (channel != null) return channel;
            Log.Error("Either the guild is null or the channel is null.");
            return null;
        }

        private void GetChannelAndBind(string handler, string @event, ulong channelId)
        {
            Log.Debug($"Getting channel {channelId} from {handler}.{@event}");
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

        private bool EventExists(string handler, string @event)
        {
            IPlugin<IConfig> eventsAssembly =
                Exiled.Loader.Loader.Plugins.FirstOrDefault(x => x.Name == "Exiled.Events");
            if (eventsAssembly == null) return false;
            Type eventType = eventsAssembly.Assembly.GetTypes()
                .FirstOrDefault(x => x.Namespace == "Exiled.Events.Handlers" && x.Name == handler);
            if (eventType == null) return false;
            PropertyInfo propertyInfo = eventType.GetProperty(@event);
            return propertyInfo != null;
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
            bool eventResponse = EventExists(log.Handler, log.Event);
            if (eventResponse)
            {
                logList.Add(JObject.FromObject(log));
                WriteableConfig.WriteConfigOption("AdvancedLogging", logList);
                EventManager.AddEventHandler(log.Handler, log.Event);
                await modal.RespondAsync("Log added", ephemeral: true);
            }
            else
            {
                await modal.RespondAsync("Failed to add log, check server console for more info", ephemeral: true);
            }
        }
    }
}