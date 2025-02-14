using System.Reflection;
using DiscordLab.Bot.API.Interfaces;
using LabApi.Loader;

namespace DiscordLab.Moderation.Handlers
{
    public class ModerationLogsHandler : IRegisterable
    {
        public static ModerationLogsHandler Instance { get; private set; }

        private Type HandlerType { get; set; } = null!;
        public bool IsEnabled;
        public object HandlerInstance { get; private set; } = null!;
        public MethodInfo SendBanLogMethod { get; private set; } = null!;
        public MethodInfo SendUnbanLogMethod { get; private set; } = null!;
        
        public void Init()
        {
            Instance = this;
            KeyValuePair<LabApi.Loader.Features.Plugins.Plugin,Assembly>? moderationPlugin = PluginLoader.Plugins.FirstOrDefault(p => p.Key.Name == "DiscordLab.ModerationLogs");
            if (moderationPlugin == null)
            {
                IsEnabled = false;
                return;
            }

            Assembly assembly = moderationPlugin.Value.Value;
            
            HandlerType = assembly.GetType("DiscordLab.ModerationLogs.Handlers.DiscordBot");
            HandlerInstance = HandlerType.GetProperty("Instance")!.GetValue(null);
            SendBanLogMethod = HandlerType.GetMethod("SendBanMessage")!;
            SendUnbanLogMethod = HandlerType.GetMethod("SendUnbanMessage")!;
        }

        public void Unregister()
        {
            HandlerType = null!;
            HandlerInstance = null!;
            SendBanLogMethod = null!;
            SendUnbanLogMethod = null!;
        }
    }
}