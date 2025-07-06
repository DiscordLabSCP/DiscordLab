using Discord;
using DiscordLab.Bot.API.Attributes;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Interfaces;
using LabApi.Events.CustomHandlers;
using LabApi.Features;
using LabApi.Features.Wrappers;
using LabApi.Loader;

namespace DiscordLab.Moderation
{
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
                ISlashCommand.FindAll();
            
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

        public static SlashCommandBuilder SetupDurationBuilder(SlashCommandBuilder original, bool required = false)
        {
            SlashCommandOptionBuilder playerOption = original.Options[0];
            SlashCommandOptionBuilder durationOption = original.Options[1];

            playerOption.Type = ApplicationCommandOptionType.String;
            playerOption.IsAutocomplete = true;
            playerOption.IsRequired = true;

            durationOption.Type = ApplicationCommandOptionType.String;
            durationOption.IsAutocomplete = false;
            durationOption.IsRequired = required;

            return original;
        }
    }
}