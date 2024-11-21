using System.Reflection;
using Discord.WebSocket;
using DiscordLab.AdvancedLogging.API.Features;
using DiscordLab.AdvancedLogging.API.Modules;
using DiscordLab.Bot.API.Interfaces;
using DiscordLab.Bot.API.Modules;
using Newtonsoft.Json.Linq;
using Log = Exiled.API.Features.Log;

namespace DiscordLab.AdvancedLogging.Handlers;

public class DiscordBot : IRegisterable
{
    public static DiscordBot Instance { get; private set; }
    
    private List<ChannelType> Channels { get; set; }
    
    public void Init()
    {
        Instance = this;
        Bot.Handlers.DiscordBot.Instance.Client.ModalSubmitted += OnModalSubmitted;
        Bot.Handlers.DiscordBot.Instance.Client.Ready += OnReady;
        Channels = new ();
    }
    
    public void Unregister()
    {
        Channels = null;
    }

    public SocketTextChannel GetChannel(ulong channelId)
    {
        if(Bot.Handlers.DiscordBot.Instance.Guild == null) return null;
        if(Channels.Exists(c => c.ChannelId == channelId)) return Channels.First(c => c.ChannelId == channelId).Channel;
        SocketTextChannel channel = Bot.Handlers.DiscordBot.Instance.Guild.GetTextChannel(channelId);
        if(channel == null)
        {
            Log.Error("Either the guild is null or the channel is null. So the status message has failed to send.");
            return null;
        }

        return channel;
    }

    public void GetChannelAndBind(string handler, string @event, ulong channelId)
    {
        SocketTextChannel channel = GetChannel(channelId);
        Channels.Add(new ChannelType
        {
            Handler = handler,
            Event = @event,
            Channel = channel,
            ChannelId = channelId
        });
    }

    private async Task OnReady()
    {
        JToken logs = WriteableConfig.GetConfig()["AdvancedLogging"];
        if (logs == null)
        {
            WriteableConfig.WriteConfigOption("AdvancedLogging", new JArray());
            return;
        }
        IEnumerable<API.Features.Log> logList = logs.ToObject<IEnumerable<API.Features.Log>>();
        foreach (API.Features.Log log in logList)
        {
            GetChannelAndBind(log.Handler, log.Event, log.ChannelId);
            BindEvent(log);
        }
        await Task.CompletedTask;
    }

    private void BindEvent(API.Features.Log log)
    {
        Type handlerType = Type.GetType($"Exiled.Events.Handlers.{log.Handler}");
        if (handlerType == null)
        {
            Log.Error($"Handler type 'Exiled.Events.Handlers.{log.Handler}' not found.");
            return;
        }

        EventInfo eventInfo = handlerType.GetEvent(log.Event);
        if (eventInfo == null)
        {
            Log.Error($"Event '{log.Event}' not found in handler '{log.Handler}'.");
            return;
        }

        Action<object> action = (ev) => OnEventTriggered(ev, log);
        Delegate handlerDelegate = Delegate.CreateDelegate(eventInfo.EventHandlerType, action.Target, action.Method);
        eventInfo.AddEventHandler(null, handlerDelegate);
    }

    private void OnEventTriggered(object ev, API.Features.Log log)
    {
        GenerateEvent.Event(ev, GetChannel(log.ChannelId), log.Content, (log.Nullables ?? "").Split(','));
    }

    private async Task OnModalSubmitted(SocketModal modal)
    {
        if(modal.Data.CustomId != "addlogmodal") return;
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
            await modal.RespondAsync("Either the guild is null or the channel is null. So I couldn't find the channel you linked.", ephemeral:true);
        }
        Channels.Add(new ChannelType
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
        JArray logList = logs.ToObject<JArray>();
        API.Features.Log log = new()
        {
            Handler = handler,
            Event = @event,
            Content = message,
            Nullables = nullables ?? "",
            ChannelId = channelId
        };
        logList.Add(JObject.FromObject(log));
        WriteableConfig.WriteConfigOption("AdvancedLogging", logList);
        BindEvent(log);
        await modal.RespondAsync("Log added", ephemeral:true);
    }
}