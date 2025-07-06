namespace DiscordLab.Bot.API.Features
{
    using LabApi.Loader;

    /// <summary>
    /// Allows users to easily make plugins with translations also, not just configs.
    /// </summary>
    /// <typeparam name="TConfig">Your config.</typeparam>
    /// <typeparam name="TTranslation">Your translation.</typeparam>
    public abstract class Plugin<TConfig, TTranslation> : LabApi.Loader.Features.Plugins.Plugin
        where TConfig : class, new()
        where TTranslation : class, new()
    {
        /// <summary>
        /// Gets the plugin's config.
        /// </summary>
        public TConfig Config;

        /// <summary>
        /// Gets the plugin's translation.
        /// </summary>
        public TTranslation Translation;

        /// <inheritdoc/>
        public override void LoadConfigs()
        {
            this.TryLoadConfig("config.yml", out Config);
            this.TryLoadConfig("translation.yml", out Translation);

            base.LoadConfigs();
        }
    }
}