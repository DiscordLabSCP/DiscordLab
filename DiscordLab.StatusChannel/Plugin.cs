using DiscordLab.Bot.API.Attributes;
using DiscordLab.Bot.API.Features;
using DiscordLab.Dependency;
using LabApi.Events.CustomHandlers;
using LabApi.Events.Handlers;
using LabApi.Features;
using LabApi.Loader;

namespace DiscordLab.StatusChannel
{
    public class Plugin : Plugin<Config, Translation>
    {
        public static Plugin Instance;
        
        public override string Name { get; } = "DiscordLab.StatusChannel";
        public override string Description { get; } = "Allows you to update/send a status message in a specific channel and have it update automatically.";
        public override string Author { get; } = "LumiFae";
        public override Version Version { get; } = typeof(Plugin).Assembly.GetName().Version;
        public override Version RequiredApiVersion { get; } = new(LabApiProperties.CompiledVersion);
        
        public MessageConfig MessageConfig { get; set; }

        public Events Events = new();
        
        public override void Enable()
        {
            Instance = this;
            
            CallOnReadyAttribute.Load();
            
            CustomHandlersManager.RegisterEventsHandler(Events);
            
            if(Config.AddCommand)
                SlashCommand.FindAll();
        }

        public override void Disable()
        {
            CallOnUnloadAttribute.Unload();
            
            CustomHandlersManager.UnregisterEventsHandler(Events);

            Events = null;
            
            Instance = null;
        }

        public override void LoadConfigs()
        {
            this.TryLoadConfig("message_config.yml", out MessageConfig messageConfig);
            MessageConfig = messageConfig ?? new();
            base.LoadConfigs();
        }
    }
}