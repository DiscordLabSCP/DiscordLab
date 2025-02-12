using System.Globalization;
using DiscordLab.Bot.API.Modules;
using HarmonyLib;
using LabApi.Features;
using LabApi.Features.Console;
using LabApi.Loader.Features.Plugins.Enums;

namespace DiscordLab.AdminLogs
{
    public class Plugin : LabApi.Loader.Features.Plugins.Plugin
    {
        public override string Name => "DiscordLab.AdminLogs";
        public override string Author => "LumiFae";
        public override string Description => "The AdminLogs plugin for DiscordLab";
        public override Version Version => new (1, 0, 1);

        public override Version RequiredApiVersion { get; } = new(LabApiProperties.CompiledVersion);

        public static Plugin Instance { get; private set; } = null!;
        
        private HandlerLoader _handlerLoader = null!;

        private Harmony? _harmony;
        
        public Config Config { get; set; } = null!;
        public Translation Translation { get; set; } = null!;

        public override void Enable()
        {
            Instance = this;
            
            _handlerLoader = new ();
            if(!_handlerLoader.Load()) throw new ("Failed to load module, contact us.");

            try
            {
                _harmony = new($"discordlab.adminlogs.{DateTime.Now.Ticks}");
                _harmony.PatchAll();
            }
            catch (Exception e)
            {
                Logger.Error($"An error occurred while patching: {e}");
            }
        }

        public override void LoadConfigs()
        {
            ConfigLoader.LoadConfigs(this, out Config config, out Translation translation);
            Translation = translation;
            Config = config;
        }

        public override void Disable()
        {
            _handlerLoader.Unload();
            _handlerLoader = null;
            
            _harmony?.UnpatchAll(_harmony.Id);
            _harmony = null;
        }
        
        public static uint GetColor(string color)
        {
            return uint.Parse(color, NumberStyles.HexNumber);
        }
    }
}