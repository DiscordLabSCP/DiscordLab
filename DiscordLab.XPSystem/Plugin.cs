using DiscordLab.Bot.API.Interfaces;
using Exiled.API.Enums;
using Exiled.API.Features;
using XPSystem;

namespace DiscordLab.XPSystem
{
    public class Plugin : Plugin<Config, Translation>
    {
        public override string Name => "DiscordLab.XPSystem";
        public override string Author => "JayXTQ";
        public override string Prefix => "DL.XPSystem";
        public override Version Version => new (1, 0, 0);
        public override Version RequiredExiledVersion => new (8, 11, 0);
        public override PluginPriority Priority => PluginPriority.Default;

        public static Plugin Instance { get; private set; }
        
        private List<IRegisterable> _inits = new();

        public override void OnEnabled()
        {
            if (Main.Instance.Version < new Version(2, 0, 8))
            {
                Log.Error("DiscordLab.XPSystem requires XPSystem to be at least version 2.0.8!");
                return;
            }

            Instance = this;
            
            Type registerType = typeof(IRegisterable);
            foreach (Type type in Assembly.GetTypes())
            {
                if (type.IsAbstract || !registerType.IsAssignableFrom(type))
                    continue;

                IRegisterable init = Activator.CreateInstance(type) as IRegisterable;
                _inits.Add(init);
                init!.Init();
            }
            
            base.OnEnabled();
        }
        
        public override void OnDisabled()
        {
            foreach (IRegisterable init in _inits)
                init.Unregister();
            
            base.OnDisabled();
        }
    }
}