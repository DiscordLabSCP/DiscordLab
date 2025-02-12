using DiscordLab.Bot.API.Interfaces;
using LabApi.Loader;

namespace DiscordLab.Bot.API.Modules
{
    public static class ConfigLoader
    {
        public static void LoadConfigs<TConfig, TTranslation>(LabApi.Loader.Features.Plugins.Plugin instance, out TConfig config, out TTranslation translation)
            where TConfig : class, IConfig, new()
            where TTranslation : class, IConfig, new()
        {
            config = instance.TryLoadConfig("config.yml", out TConfig? tempConfig) ? tempConfig : new ();
            translation = instance.TryLoadConfig("translation.yml", out TTranslation? tempTranslation) ? tempTranslation : new ();
        }

        public static void LoadConfigs<TConfig>(LabApi.Loader.Features.Plugins.Plugin instance, out TConfig config)
            where TConfig : class, IConfig, new()
        {
            config = instance.TryLoadConfig("config.yml", out TConfig? tempConfig) ? tempConfig : new ();
        }
    }
}