using DiscordLab.Core.API.Attributes;
using DiscordLab.Core.API.Commands;
using LabApi.Loader;

namespace DiscordLab.Bot;

using Discord;
using HarmonyLib;
using LabApi.Features;
using LabApi.Features.Console;
using LabApi.Loader.Features.Plugins;
using LabApi.Loader.Features.Plugins.Enums;

/// <inheritdoc />
public sealed class Plugin : Plugin<Config>
{
    /// <summary>
    /// Gets the current instance of this plugin.
    /// </summary>
    public static Plugin Instance { get; private set; } = null!;

    /// <inheritdoc />
    public override string Name { get; } = "DiscordLab";

    /// <inheritdoc />
    public override string Description { get; } = "A modular Discord bot for SCP:SL servers running LabAPI";

    /// <inheritdoc />
    public override string Author { get; } = "LumiFae";

    /// <inheritdoc />
    public override Version Version => GetType().Assembly.GetName().Version;

    /// <inheritdoc />
    public override Version RequiredApiVersion { get; } = new(LabApiProperties.CompiledVersion);

    /// <inheritdoc />
    public override LoadPriority Priority { get; } = LoadPriority.Highest;

    private Harmony Harmony { get; } = new($"DiscordLab.Bot-{DateTime.Now.Ticks}");

    public MessageConfig MessageConfig { get; private set; } = null!;

    /// <inheritdoc />
    public override void Enable()
    {
        Instance = this;
        CommandHandler.Instance = new();

        try
        {
            TokenUtils.ValidateToken(TokenType.Bot, Config.Token);
        }
        catch (Exception)
        {
            Logger.Error("DiscordLab bot token is invalid");
            return;
        }

        Harmony.PatchAll();

        CallOnLoadAttribute.Load();
    }

    /// <inheritdoc />
    public override void Disable()
    {
        Harmony.UnpatchAll();

        CallOnUnloadAttribute.Unload();

        Instance = null!;
        MessageConfig = null!;
        CommandHandler.Instance = null!;
    }

    public override void LoadConfigs()
    {
        MessageConfig = this.LoadConfig<MessageConfig>("message-config.yml") ?? new();
        
        base.LoadConfigs();
    }
}