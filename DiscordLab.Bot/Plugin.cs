using DiscordLab.Bot.API.Modules;
using Exiled.API.Enums;
using Exiled.API.Features;

namespace DiscordLab.Bot
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "DiscordLab";
        public override string Author => "LumiFae";
        public override string Prefix => "DiscordLab";
        public override Version Version => new (1, 5, 1);
        public override Version RequiredExiledVersion => new (8, 11, 0);
        public override PluginPriority Priority => PluginPriority.Higher;

        public static Plugin Instance { get; private set; }
        
        private HandlerLoader _handlerLoader;

        public override void OnEnabled()
        {
            Instance = this;
            
            if(Config.Token is "token" or "")
            {
                Log.Error("Please set the bot token in the config file.");
                return;
            }

            if (Config.GuildId is 0)
            {
                Log.Warn("You have no guild ID set in the config file, you might get errors until you set it. " +
                         "If you plan on having guild IDs separate for every module then you can ignore this. " +
                         "For more info go to here: https://github.com/DiscordLabSCP/DiscordLab/wiki/Installation#guild-id");
            }
            
            _handlerLoader = new ();
            _handlerLoader.Load(Assembly);

            Task.Run(UpdateStatus.GetStatus);
            
            base.OnEnabled();
        }
        
        public override void OnDisabled()
        {
            _handlerLoader.Unload();
            _handlerLoader = null;
            
            SlashCommandLoader.ClearCommands();
            
            base.OnDisabled();
        }
    }
}