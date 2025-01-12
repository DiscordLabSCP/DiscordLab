using System.Reflection;
using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Features;
using Exiled.API.Interfaces;
using Exiled.Loader;

namespace DiscordLab.Moderation.Handlers
{
    public class ModerationLogsHandler : IRegisterable
    {
        public static ModerationLogsHandler Instance { get; private set; }
        
        private Type HandlerType { get; set; }
        public bool IsEnabled;
        public object HandlerInstance { get; private set; }
        public MethodInfo SendBanLogMethod { get; private set; }
        public MethodInfo SendUnbanLogMethod { get; private set; }
        
        public void Init()
        {
            Instance = this;
            IPlugin<IConfig> moderationPlugin = Loader.Plugins.FirstOrDefault(p => p.Name == "DiscordLab.ModerationLogs");
            if (moderationPlugin == null)
            {
                IsEnabled = false;
                return;
            }

            IsEnabled = moderationPlugin.Config.IsEnabled;

            Assembly assembly = moderationPlugin.Assembly;
            
            HandlerType = assembly.GetType("DiscordLab.ModerationLogs.Handlers.DiscordBot");
            HandlerInstance = HandlerType.GetProperty("Instance")!.GetValue(null);
            SendBanLogMethod = HandlerType.GetMethod("SendBanMessage")!;
            SendUnbanLogMethod = HandlerType.GetMethod("SendUnbanMessage")!;
        }

        public void Unregister()
        {
            HandlerType = null;
            HandlerInstance = null;
            SendBanLogMethod = null;
            SendUnbanLogMethod = null;
        }
    }
}