using Discord;
using DiscordLab.Bot.API.Attributes;
using DiscordLab.Bot.API.Features;
using DiscordLab.Dependency;
using DiscordLab.Moderation.Commands;
using LabApi.Events.CustomHandlers;
using LabApi.Features;
using LabApi.Features.Wrappers;
using LabApi.Loader;
using RemoteAdmin;

namespace DiscordLab.Moderation;

public class Plugin : Plugin<Config, Translation>
{
    public static Plugin Instance;
        
    public override string Name { get; } = "DiscordLab.Moderation";
    public override string Description { get; } = "Adds logging and commands for moderation based operations";
    public override string Author { get; } = "LumiFae";
    public override Version Version { get; } = typeof(Plugin).Assembly.GetName().Version;
    public override Version RequiredApiVersion { get; } = new(LabApiProperties.CompiledVersion);

    public TempMuteConfig MuteConfig;

    public Events Events = new();
        
    public override void Enable()
    {
        Instance = this;
            
        CallOnLoadAttribute.Load();
            
        if (Config.AddCommands)
            SlashCommand.FindAll();
            
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

    public static IEnumerable<AutocompleteResult> PlayersAutocompleteResults =>
        Player.ReadyList.Select(p => new AutocompleteResult(p.Nickname, p.PlayerId));
}