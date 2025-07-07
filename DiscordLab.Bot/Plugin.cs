namespace DiscordLab.Bot
{
    using DiscordLab.Bot.API.Attributes;
    using DiscordLab.Bot.API.Features;
    using HarmonyLib;
    using LabApi.Features;
    using LabApi.Loader.Features.Plugins;
    using LabApi.Loader.Features.Plugins.Enums;

    /// <inheritdoc />
    public sealed class Plugin : Plugin<Config>
    {
        /// <summary>
        /// Gets the current instance of this plugin.
        /// </summary>
        public static Plugin Instance { get; private set; }

        /// <inheritdoc />
        public override string Name { get; } = "DiscordLab";

        /// <inheritdoc />
        public override string Description { get; } = "A modular Discord bot for SCP:SL servers running LabAPI";

        /// <inheritdoc />
        public override string Author { get; } = "LumiFae";

        /// <inheritdoc />
        public override Version Version { get; } = typeof(Plugin).Assembly.GetName().Version;

        /// <inheritdoc />
        public override Version RequiredApiVersion { get; } = new(LabApiProperties.CompiledVersion);

        /// <inheritdoc />
        public override LoadPriority Priority { get; } = LoadPriority.Highest;

        /// <summary>
        /// Gets the current config for the plugin.
        /// </summary>
        public new Config Config { get; private set; }

        private Harmony Harmony { get; } = new($"DiscordLab.Bot-{DateTime.Now.Ticks}");

        /// <inheritdoc />
        public override void Enable()
        {
            Harmony.PatchAll();

            Instance = this;
            Config = base.Config;

            CallOnLoadAttribute.Load();
            CallOnReadyAttribute.Load();

            SlashCommand.FindAll();
        }

        /// <inheritdoc />
        public override void Disable()
        {
            Harmony.UnpatchAll();

            CallOnUnloadAttribute.Unload();

            Config = null;
            Instance = null;
        }
    }
}