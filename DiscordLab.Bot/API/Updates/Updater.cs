namespace DiscordLab.Bot.API.Updates;

using System.Net.Http;
using DiscordLab.Bot.API.Attributes;
using LabApi.Features.Console;
using LabApi.Loader;
using Utf8Json;

/// <summary>
/// Handle updates within DiscordLab.
/// </summary>
public static class Updater
{
    /// <summary>
    /// Gets the HTTP Client to use for checking for releases.
    /// </summary>
    public static HttpClient Client { get; private set; } = new();

    /// <summary>
    /// Gets the HTTP Client to use for downloading new modules.
    /// </summary>
    public static HttpClient DownloadClient { get; private set; } = new();

    /// <summary>
    /// Checks the GitHub repository for updates, and returns back the latest versions of each module.
    /// </summary>
    /// <returns>The latest versions of each module.</returns>
    public static async Task<IReadOnlyCollection<Module>> CheckForUpdates()
    {
        using HttpResponseMessage response = await Client.GetAsync(string.Empty);

        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (Exception)
        {
            return [];
        }

        using Stream stream = await response.Content.ReadAsStreamAsync();

        GitHubRelease[] releases = JsonSerializer.Deserialize<GitHubRelease[]>(stream);

        List<Module> modules = [];

        foreach (GitHubRelease release in releases)
        {
            if (release.Prerelease || release.Draft)
                continue;
            if (release.TagName.Count(c => c == '.') == 3)
                continue;
            Version version = new(release.TagName.Split('-').First());

            if (version.Major != Plugin.Instance.Version.Major)
                continue;

            foreach (GitHubReleaseAsset asset in from asset in release.Assets let name = asset.Name.Replace(".dll", string.Empty) where !modules.Any(module => module.Name == name && module.Version > version) select asset)
            {
                modules.Add(new(release, asset));
            }
        }

        Module.CurrentModules = modules;

        return Module.CurrentModules;
    }

    /// <summary>
    /// Runs <see cref="CheckForUpdates"/> and will either log out the updates available or install the updates, it depends on the config values.
    /// </summary>
    /// <returns>The modules that were updated, or need updating.</returns>
    public static async Task<IReadOnlyCollection<Module>> ManageUpdates()
    {
        IEnumerable<Module> modules = await CheckForUpdates();
        List<Module> modulesToUpdate = [];

        foreach (Module module in modules)
        {
            if (module.ExistingPlugin == null)
                continue;

            if (module.Version > module.ExistingPlugin.Version)
                modulesToUpdate.Add(module);
        }

        if (modulesToUpdate.Count == 0)
            return [];

        Logger.Warn($"DiscordLab modules need updating:\n${Module.GenerateUpdateString(modulesToUpdate)}");

        if (!Plugin.Instance.Config.AutoUpdate)
            return modulesToUpdate;

        Logger.Info("Downloading DiscordLab updates...");

        foreach (Module module in modulesToUpdate)
        {
            await module.Download();
        }

        Logger.Info("All DiscordLab modules updated...");

        return modulesToUpdate;
    }

    [CallOnLoad]
    private static void Setup()
    {
        Client.BaseAddress = new("https://api.github.com/repos/DiscordLabSCP/DiscordLab/releases");
        Client.DefaultRequestHeaders.Add("User-Agent", $"DiscordLab/{Plugin.Instance.Version}");

        if (Plugin.Instance.Config.AutoUpdate)
        {
            DownloadClient.BaseAddress = new("https://github.com/DiscordLabSCP/DiscordLab/releases/download");
            DownloadClient.DefaultRequestHeaders.Add("User-Agent", $"DiscordLab/{Plugin.Instance.Version}");
        }

        Task.Run(ManageUpdates);
    }

    [CallOnUnload]
    private static void Disable()
    {
        Client.Dispose();
        DownloadClient.Dispose();
    }
}