using System.Globalization;
using DiscordLab.Bot.API.Modules;
using HarmonyLib;
using LabApi.Features;
using LabApi.Features.Console;

namespace DiscordLab.ModerationLogs
{
    public class Plugin : LabApi.Loader.Features.Plugins.Plugin
    {
        public override string Name => "DiscordLab.ModerationLogs";
        public override string Author => "LumiFae";
        public override string Description => "ModerationLogs module for DiscordLab";
        public override Version Version => new (1, 5, 1);
        public override Version RequiredApiVersion => new (LabApiProperties.CompiledVersion);

        public static Plugin Instance { get; private set; } = null!;
        
        private HandlerLoader _handlerLoader = null!;

        private Harmony harmony = null!;
        
        public Config Config { get; set; } = null!;
        public Translation Translation { get; set; } = null!;

        public override void Enable()
        {
            Instance = this;
            
            _handlerLoader = new ();
            if(!_handlerLoader.Load()) throw new Exception("Failed to load module, contact us");

            try
            {
                harmony = new($"discordlab.moderationlogs.{DateTime.Now.Ticks}");
                harmony.PatchAll();
            }
            catch (Exception e)
            {
                Logger.Error($"An error occurred while patching: {e}");
            }
        }
        
        public override void Disable()
        {
            _handlerLoader.Unload();
            _handlerLoader = null!;
            
            harmony?.UnpatchAll(harmony.Id);
            harmony = null!;
        }

        public override void LoadConfigs()
        {
            this.LoadConfigs(out Config config, out Translation translation);
            Config = config;
            Translation = translation;
        }

        public static uint GetColor(string color)
        {
            return uint.Parse(color, NumberStyles.HexNumber);
        }
    }
}