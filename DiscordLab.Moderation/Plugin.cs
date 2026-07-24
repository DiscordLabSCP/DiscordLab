using DiscordLab.Core.API.Attributes;
using DiscordLab.Core.API.Commands;
using DiscordLab.Core.API.Features;
using DiscordLab.Moderation.Commands;
using LabApi.Events.CustomHandlers;
using LabApi.Features;
using LabApi.Loader;
using RemoteAdmin;

namespace DiscordLab.Moderation;

public class Plugin : Plugin<Config, Translation>
{
    public static Plugin Instance;

    public override string Name { get; } = "DiscordLab.Moderation";
    public override string Description { get; } = "Adds logging and commands for moderation based operations";
    public override string Author { get; } = "LumiFae";
    public override Version Version => GetType().Assembly.GetName().Version;
    public override Version RequiredApiVersion { get; } = new(LabApiProperties.CompiledVersion);

    public TempMuteConfig MuteConfig;

    public Events Events = new();

    public override void Enable()
    {
        Instance = this;

        CallOnLoadAttribute.Load();

        if (Config.AddCommands)
            ICommand.FindAll();

        if (Config.AddTempMuteCommand)
            CommandProcessor.RemoteAdminCommandHandler.RegisterCommand(new TempMuteRemoteAdmin());

        CustomHandlersManager.RegisterEventsHandler(Events);
    }

    public override void Disable()
    {
        CustomHandlersManager.UnregisterEventsHandler(Events);

        CallOnUnloadAttribute.Unload();

        Events = null;

        Instance = null;
    }

    public override void LoadConfigs()
    {
        this.TryLoadConfig("mute-config.yml", out MuteConfig);

        base.LoadConfigs();
    }
}