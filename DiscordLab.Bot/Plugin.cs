using Discord;
using DiscordLab.Bot.API.Modules;
using Exiled.API.Enums;
using Exiled.API.Features;
using GameCore;
using Log = Exiled.API.Features.Log;
using Version = System.Version;

namespace DiscordLab.Bot
{
    public class Plugin : Plugin<Config>
    {
        public override string Name => "DiscordLab";
        public override string Author => "LumiFae";
        public override string Prefix => "DiscordLab";
        public override Version Version => new (1, 5, 4);
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

            try
            {
                TokenUtils.ValidateToken(TokenType.Bot, Config.Token);
            }
            catch (Exception)
            {
                Log.Error("Token is invalid, please put the correct token in the config file.");
                return;
            }

            if (Config.GuildId is 0)
            {
                Log.Warn("You have no guild ID set in the config file, you might get errors until you set it. " +
                         "If you plan on having guild IDs separate for every module then you can ignore this. " +
                         "For more info go to here: https://github.com/DiscordLabSCP/DiscordLab/wiki/Installation#guild-id");
            }
            
            string restartAfterRoundsConfig = ConfigFile.ServerConfig.GetString("restart_after_rounds", "0");

            if (int.TryParse(restartAfterRoundsConfig, out int restartAfterRounds) &&
                restartAfterRounds is >= 1 and < 10)
            {
                Log.Warn("You have a restart_after_rounds value set between 1 and 9, which isn't recommended. DiscordLab restarts every time your server restarts, so it's recommended" +
                         "to set a high number, or 0, for this value to avoid potential Discord rate limits. This is just a warning.");
            }
            
            SlashCommandLoader.Create();
            
            _handlerLoader = new ();
            _handlerLoader.Load(Assembly);

            Task.Run(UpdateStatus.GetStatus);
            
            base.OnEnabled();
        }
        
        public override void OnDisabled()
        {
            _handlerLoader.Unload();
            _handlerLoader = null;
            
            SlashCommandLoader.Destroy();
            
            base.OnDisabled();
        }
    }
}