using LabApi.Loader;

namespace DiscordLab.Dependency;

/// <summary>
/// Allows users to easily make plugins with translations also, not just configs.
/// </summary>
/// <typeparam name="TConfig">Your config.</typeparam>
/// <typeparam name="TTranslation">Your translation.</typeparam>
public abstract class Plugin<TConfig, TTranslation> : LabApi.Loader.Features.Plugins.Plugin
    where TConfig : class, new()
    where TTranslation : class, new()
{
#pragma warning disable SA1401 // FieldsMustBePrivate
    /// <summary>
    /// Gets the plugin's config.
    /// </summary>
    public TConfig Config;

    /// <summary>
    /// Gets the plugin's translation.
    /// </summary>
    public TTranslation Translation;
#pragma warning restore SA1401 // FieldsMustBePrivate

    /// <inheritdoc/>
    public override void LoadConfigs()
    {
        this.TryLoadConfig("config.yml", out Config);
        this.TryLoadConfig("translation.yml", out Translation);

        base.LoadConfigs();
    }
}