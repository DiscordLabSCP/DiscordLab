using System.Globalization;
using DiscordLab.Bot.API.Modules;
using Exiled.API.Enums;
using Exiled.API.Features;
using HarmonyLib;

namespace DiscordLab.AdminLogs
{
    public class Plugin : Plugin<Config, Translation>
    {
        public override string Name => "DiscordLab.AdminLogs";
        public override string Author => "LumiFae";
        public override string Prefix => "DL.AdminLogs";
        public override Version Version => new (1, 0, 1);
        public override Version RequiredExiledVersion => new (8, 11, 0);
        public override PluginPriority Priority => PluginPriority.Default;

        public static Plugin Instance { get; private set; }
        
        private HandlerLoader _handlerLoader;

        private Harmony harmony;

        public override void OnEnabled()
        {
            Instance = this;
            
            _handlerLoader = new ();
            if(!_handlerLoader.Load(Assembly)) return;

            try
            {
                harmony = new($"discordlab.adminlogs.{DateTime.Now.Ticks}");
                harmony.PatchAll();
            }
            catch (Exception e)
            {
                Log.Error($"An error occurred while patching: {e}");
            }

            base.OnEnabled();
        }
        
        public override void OnDisabled()
        {
            _handlerLoader.Unload();
            _handlerLoader = null;
            
            harmony?.UnpatchAll(harmony.Id);
            harmony = null;
            
            base.OnDisabled();
        }
        
        public static uint GetColor(string color)
        {
            return uint.Parse(color, NumberStyles.HexNumber);
        }
    }
}