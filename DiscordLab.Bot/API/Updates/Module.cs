namespace DiscordLab.Bot.API.Updates;

using LabApi.Features.Console;
using LabApi.Loader;
using LabApi.Loader.Features.Paths;

/// <summary>
/// Contains information about a DiscordLab module.
/// </summary>
public class Module
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Module"/> class.
    /// </summary>
    /// <param name="release">The release of this module.</param>
    /// <param name="asset">The asset of this module.</param>
    public Module(GitHubRelease release, GitHubReleaseAsset asset)
    {
        Release = release;
        Asset = asset;
        Name = asset.Name.Replace(".dll", string.Empty);
        Version = new(release.TagName.Split('-').First());
        ExistingPlugin =
            PluginLoader.Plugins.Keys.FirstOrDefault(x =>
                Name == "DiscordLab.Bot" ? x.Name == "DiscordLab" : x.Name == Name);
    }

    /// <summary>
    /// Gets the found modules as of current.
    /// </summary>
    public static IReadOnlyCollection<Module> CurrentModules { get; internal set; } = [];

    /// <summary>
    /// Gets the module name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the version of this Update.
    /// </summary>
    public Version Version { get; }

    /// <summary>
    /// Gets the plugin of this module, null if module is not installed.
    /// </summary>
    public LabApi.Loader.Features.Plugins.Plugin? ExistingPlugin { get; }

    /// <summary>
    /// Gets the release this module comes from.
    /// </summary>
    public GitHubRelease Release { get; }

    /// <summary>
    /// Gets the asset this module comes from.
    /// </summary>
    public GitHubReleaseAsset Asset { get; }

    /// <summary>
    /// Generates a string used to show current version and latest version for a list of modules. Will throw if no existing plugin.
    /// </summary>
    /// <param name="modules">The modules to generate for.</param>
    /// <returns>The generated string.</returns>
    public static string GenerateUpdateString(IEnumerable<Module> modules) => string.Join(
        "\n- ",
        modules.Select(module =>
            $"{module.Name} | Current Version: {module.ExistingPlugin!.Version} | Latest Version: {module.Version}"));

    /// <summary>
    /// Downloads this module.
    /// </summary>
    /// <returns>A <see cref="Task"/>.</returns>
    internal async Task Download()
    {
        string filePath = Path.Combine(PathManager.Plugins.FullName, "global", Asset.Name);

        byte[] data;

        try
        {
            data = await Updater.DownloadClient.GetByteArrayAsync($"{Release.TagName}/{Asset.Name}");
        }
        catch (Exception ex)
        {
            Logger.Error($"Had an error whilst updating {Name} at version {Version}:\n{ex}");
            throw;
        }

        if (ExistingPlugin != null)
        {
            filePath = Path.Combine(Path.GetDirectoryName(ExistingPlugin.FilePath)!, Asset.Name);
            File.Delete(ExistingPlugin.FilePath);
        }

        await File.WriteAllBytesAsync(filePath, data);
    }
}