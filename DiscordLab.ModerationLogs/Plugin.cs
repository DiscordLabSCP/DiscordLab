using System.Globalization;
using DiscordLab.Bot.API.Modules;
using Exiled.API.Enums;
using Exiled.API.Features;
using HarmonyLib;

namespace DiscordLab.ModerationLogs
{
    public class Plugin : Plugin<Config, Translation>
    {
        public override string Name => "DiscordLab.ModerationLogs";
        public override string Author => "JayXTQ";
        public override string Prefix => "DL.ModerationLogs";
        public override Version Version => new (1, 4, 2);
        public override Version RequiredExiledVersion => new (8, 11, 0);
        public override PluginPriority Priority => PluginPriority.Default;

        public static Plugin Instance { get; private set; }
        
        private HandlerLoader _handlerLoader;

        private Harmony harmony;

        public override void OnEnabled()
        {
            Instance = this;
            
            _handlerLoader = new ();
            _handlerLoader.Load(Assembly);

            try
            {
                harmony = new($"discordlab.moderationlogs.{DateTime.Now.Ticks}");
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