using DiscordLab.Bot.API.Modules;
using LabApi.Features;
using LabApi.Features.Console;
using LabApi.Loader;
using LabApi.Loader.Features.Plugins;
using LabApi.Loader.Features.Plugins.Enums;
using Version = System.Version;

namespace DiscordLab.Bot
{
    public class Plugin : LabApi.Loader.Features.Plugins.Plugin
    {
        public override string Name => "DiscordLab";
        public override string Author => "LumiFae";
        public override string Description => "A modular Discord Bot solution.";
        public override Version Version => new (1, 5, 2);

        public override Version RequiredApiVersion { get; } = new(LabApiProperties.CompiledVersion);
        public override LoadPriority Priority => LoadPriority.Highest;

        public static Plugin Instance { get; private set; } = null!;

        public Config Config { get; private set; } = null!;
        
        private HandlerLoader _handlerLoader = null!;

        public override void Enable()
        {
            Instance = this;
            
            if(Config.Token is "token" or "")
            {
                Logger.Error("Please set the bot token in the config file.");
                return;
            }

            if (Config.GuildId is 0)
            {
                Logger.Warn("You have no guild ID set in the config file, you might get errors until you set it. " +
                         "If you plan on having guild IDs separate for every module then you can ignore this. " +
                         "For more info go to here: https://github.com/DiscordLabSCP/DiscordLab/wiki/Installation#guild-id");
            }
            
            string restartAfterRoundsConfig = GameCore.ConfigFile.ServerConfig.GetString("restart_after_rounds", "0");

            if (int.TryParse(restartAfterRoundsConfig, out int restartAfterRounds) &&
                restartAfterRounds is >= 1 and < 10)
            {
                Logger.Warn("You have a restart_after_rounds value set between 1 and 9, which isn't recommended. DiscordLab restarts every time your server restarts, so it's recommended" +
                         "to set a high number, or 0, for this value to avoid potential Discord rate limits. This is just a warning.");
            }
            
            _handlerLoader = new ();
            _handlerLoader.Load();

            Task.Run(UpdateStatus.GetStatus);
        }

        public override void LoadConfigs()
        {
            ConfigLoader.LoadConfigs(this, out Config config);
            Config = config;
        }
        
        public override void Disable()
        {
            _handlerLoader.Unload();
            _handlerLoader = null!;
            
            SlashCommandLoader.ClearCommands();
        }
    }
}