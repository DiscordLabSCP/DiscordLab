using System.Reflection;
using Discord.WebSocket;
using DiscordLab.AdvancedLogging.API.Features;
using DiscordLab.Bot.API.Interfaces;

namespace DiscordLab.AdvancedLogging.Handlers;

public class DiscordBot : IRegisterable
{
    public static DiscordBot Instance { get; private set; }

    private List<Channel> Channels { get; set; }
    
    public void Init()
    {
        Instance = this;
        Channels = new();
    }
    
    public void Unregister()
    {
        Channels = null;
    }
    
    public SocketTextChannel GetChannel(string handler, string eventName)
    {
        Channel channel = Channels.FirstOrDefault(c => c.Handler == handler && c.EventName == eventName);
        if (channel == null) 
        {
            ulong id = GetChannelId(handler, eventName);
            if (id == 0)
            {
                return null;
            }
            Channel newChannel = new Channel
            {
                Handler = handler,
                EventName = eventName,
                ChannelId = id
            };
            Channels.Add(newChannel);
            channel = newChannel;
        };
        
        if (channel.TextChannel != null)
        {
            return channel.TextChannel;
        }

        if (channel.ChannelId == 0)
        {
            return null;
        }

        channel.TextChannel = Bot.Handlers.DiscordBot.Instance.Guild.GetTextChannel(channel.ChannelId);
        return channel.TextChannel;
    }

    public ulong GetChannelId(string handler, string eventName)
    {
        Config config = Plugin.Instance.Config;
        PropertyInfo property = config.GetType().GetProperty(handler);
        if (property == null)
        {
            return 0;
        }
        
        object handlerInstance = property.GetValue(config);
        PropertyInfo eventProperty = handlerInstance.GetType().GetProperty(eventName);
        if (eventProperty == null)
        {
            return 0;
        }
        
        return (ulong) eventProperty.GetValue(handlerInstance);
    }

    public SocketTextChannel GetChannelById(ulong id, string handler, string eventName)
    {
        Channel channel = Channels
            .FirstOrDefault(c => c.Handler == handler && c.EventName == eventName && c.ChannelId == id);
        if (channel == null)
        {
            channel = new()
            {
                Handler = handler,
                EventName = eventName,
                ChannelId = id
            };
            Channels.Add(channel);
        }

        if (channel.TextChannel != null) return channel.TextChannel;
        
        channel.TextChannel = Bot.Handlers.DiscordBot.Instance.Guild.GetTextChannel(id);
        
        return channel.TextChannel;
    }
}