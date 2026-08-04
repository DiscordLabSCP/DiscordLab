using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using DiscordLab.Core.API.Commands;
using DiscordLab.Core.API.Updater;

namespace DiscordLab.Bot.Commands.Management;

[Group("discordlab", "DiscordLab related commands")]
public class DiscordCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("list", "Lists available DiscordLab modules")]
    public async Task ListCommand()
    {
        string modules = string.Join(
            "\n",
            Module.CurrentModules.Where(s => s.Name != "DiscordLab.Bot")
                .Select(s => $"{s.Name} (v{s.Version})"));
        await RespondAsync("List of available DiscordLab modules:\n\n" + modules);
    }

    [AutocompleteCommand("module", "install")]
    public async Task Autocomplete()
    {
        if (Context.Interaction is not SocketAutocompleteInteraction autocomplete)
            return;
        
        string value = autocomplete.Data.Current.Value.ToString();

        await autocomplete.RespondAsync(Module.CurrentModules.Where(s =>
            s.Name != "DiscordLab.Bot" && s.Name.Contains(value, StringComparison.CurrentCultureIgnoreCase)).Take(25).Select(s => new AutocompleteResult(s.Name, s.Name)));
    }

    [SlashCommand("install", "Install a DiscordLab module")]
    public async Task InstallModule([Summary(description: "The module to install"), Autocomplete] string module)
    {
        if (string.IsNullOrWhiteSpace(module))
        {
            await RespondAsync("Please provide a module name.");
            return;
        }

        await DeferAsync();

        Module? moduleInstance =
            Module.CurrentModules.FirstOrDefault(s =>
                string.Equals(s.Name, module, StringComparison.CurrentCultureIgnoreCase)) ??
            Module.CurrentModules.FirstOrDefault(s =>
                s.Name.Split('.').Last().Equals(module, StringComparison.CurrentCultureIgnoreCase));
        if (moduleInstance == null || moduleInstance.Name == "DiscordLab.Bot")
        {
            await FollowupAsync("Module not found.");
            return;
        }

        await moduleInstance.Download();
        ServerStatic.StopNextRound = ServerStatic.NextRoundAction.Restart;
        await FollowupAsync("Downloaded module. Server will restart next round.");
    }

    [SlashCommand("check", "Check for DiscordLab updates")]
    public async Task Check()
    {
        await DeferAsync();
        IEnumerable<Module> modules = await Updater.ManageUpdates();
        if (!modules.Any())
        {
            await FollowupAsync("No updates found.");
            return;
        }

        await FollowupAsync($"Updates found, modules that need updating:\n{Module.GenerateUpdateString(modules)}");
    }

    [SlashCommand("update", "Update DiscordLab if possible")]
    public async Task Update()
    {
        await DeferAsync();
        IEnumerable<Module> modules = await Updater.ManageUpdates();

        if (!modules.Any())
        {
            await FollowupAsync("No updates found.");
            return;
        }

        if (!Core.Plugin.Instance.Config.AutoUpdate)
        {
            await FollowupAsync($"Updates found, modules that need updating:\n{Module.GenerateUpdateString(modules)}");
            return;
        }

        // Force updates, because ManageUpdates checks for AutoUpdate, and will trigger the update.
        foreach (Module module in modules)
        {
            await module.Download();
        }

        await FollowupAsync($"Updates found, modules that need updating:\n{Module.GenerateUpdateString(modules)}");
    }
}