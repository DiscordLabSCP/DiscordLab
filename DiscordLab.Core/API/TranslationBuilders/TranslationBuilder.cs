// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global

using System.Globalization;
using System.Text.RegularExpressions;
using DiscordLab.Core.API.Extensions;
using LabApi.Features.Wrappers;
using LightContainmentZoneDecontamination;
using RoundRestarting;
using UnityEngine;

namespace DiscordLab.Core.API.TranslationBuilders;

/// <summary>
/// Allows you to create translations with placeholders being replaced.
/// </summary>
public class TranslationBuilder
{
    private static readonly Regex TagRemoveRegex = new("<[^>]+>", RegexOptions.Compiled);

    private static readonly Regex UselessTextRemoveRegex =
        new(@"<color=#00000000>(?:.*?)<\/color>", RegexOptions.Compiled);

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslationBuilder"/> class.
    /// </summary>
    public TranslationBuilder()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TranslationBuilder"/> class.
    /// </summary>
    /// <param name="translation">The translation to modify.</param>
    public TranslationBuilder(string translation)
    {
        Translation = translation;
    }

    /// <summary>
    /// Gets the dictionary of replacers that have no argument.
    /// </summary>
    public static Dictionary<Regex, Func<string>> StaticReplacers { get; } = new()
    {
        // Map Replacers
        [CreateRegex("seed")] = static () => Map.Seed.ToString(),
        [CreateRegex("isdecont")] = static () => Decontamination.IsDecontaminating.ToString(),
        [CreateRegex("remainingdeconttime")] = GetRemainingDecontaminationTime,
        [CreateRegex("isdecontenabled")] = static () =>
            (Decontamination.Status == DecontaminationController.DecontaminationStatus.None).ToString(),
        [CreateRegex("decontstate")] = static () => Decontamination.Status.ToString(),

        // Round Replacers
        [CreateRegex("killcount")] = static () => Round.TotalDeaths.ToString(),
        [CreateRegex("elapsedtime")] = static () => Round.Duration.ToString(),
        [CreateRegex("escapedscientistscount")] = static () => Round.EscapedScientists.ToString(),
        [CreateRegex("inprogress")] = static () => Round.IsRoundInProgress.ToString(),
        [CreateRegex("isended")] = static () => Round.IsRoundEnded.ToString(),
        [CreateRegex("isstarted")] = static () => Round.IsRoundStarted.ToString(),
        [CreateRegex("islocked")] = static () => Round.IsLocked.ToString(),
        [CreateRegex("changedintozombiescount")] = static () => Round.ChangedIntoZombies.ToString(),
        [CreateRegex("escapeddclassescount")] = static () => Round.EscapedClassD.ToString(),
        [CreateRegex("islobbylocked")] = static () => Round.IsLobbyLocked.ToString(),
        [CreateRegex("scpkillcount")] = static () => Round.KilledBySCPs.ToString(),
        [CreateRegex("alivescpcount")] = static () => Round.SurvivingSCPs.ToString(),
        [CreateRegex("roundcount")] = static () => RoundRestart.UptimeRounds.ToString(),

        // Server Replacers
        [CreateRegex("maxplayers")] = static () => Server.MaxPlayers.ToString(),
        [CreateRegex("name")] = static () => Server.ServerListName,
        [CreateRegex("nameparsed")] = static () =>
        {
            string result = UselessTextRemoveRegex.Replace(Server.ServerListName, string.Empty);
            result = TagRemoveRegex.Replace(result, string.Empty);

            return result;
        },
        [CreateRegex("port")] = static () => Server.Port.ToString(),
        [CreateRegex("ip")] = static () => Server.IpAddress,
        [CreateRegex("playercount")] = static () => Server.PlayerCount.ToString(),
        [CreateRegex("playercountnonpcs")] = static () => Player.ReadyList.Count(p => !p.IsNpc).ToString(),
        [CreateRegex("tps")] = static () => Server.Tps.ToString(CultureInfo.CurrentCulture),
        [CreateRegex("version")] = static () => GameCore.Version.VersionString,
        [CreateRegex("isbeta")] = static () => GameCore.Version.PublicBeta.ToString(),
        [CreateRegex("isfriendlyfire")] = static () => Server.FriendlyFire.ToString(),
    };

    /// <summary>
    /// Gets time based replacers. The <see cref="long"/> type is the unix timestamp. Can be got with <see cref="DateTimeOffset.ToUnixTimeSeconds"/>.
    /// </summary>
    public static Dictionary<Regex, Func<long, string>> TimeReplacers { get; } = new()
    {
        [CreateRegex("time")] = static time => $"<t:{time}>",
        [CreateRegex("timet")] = static time => $"<t:{time}:t>",
        [CreateRegex("timetlong")] = static time => $"<t:{time}:T>",
        [CreateRegex("timed")] = static time => $"<t:{time}:d>",
        [CreateRegex("timedlong")] = static time => $"<t:{time}:D>",
        [CreateRegex("timef")] = static time => $"<t:{time}:f>",
        [CreateRegex("timeflong")] = static time => $"<t:{time}:F>",
        [CreateRegex("timer")] = static time => $"<t:{time}:R>",
        [CreateRegex("elapsedtimerelative")] = static time => $"<t:{time - Round.Duration.TotalSeconds}:R>",
        [CreateRegex("roundstart")] = static time => $"<t:{time - Round.Duration.TotalSeconds}:T>",
        [CreateRegex("secondssince")] = static time => TimeSince(time).Seconds.ToString(CultureInfo.InvariantCulture),
        [CreateRegex("minutessince")] = static time => TimeSince(time).Minutes.ToString(CultureInfo.InvariantCulture),
    };

    /// <summary>
    /// Gets or sets a Dictionary of custom replacers. Key is the text to replace and value is the factory to replace with.
    /// </summary>
    public Dictionary<Regex, Func<string>> CustomReplacers { get; set; } = new();

    /// <summary>
    /// Gets or sets the time that this translation will use.
    /// </summary>
    public DateTime Time { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the translation.
    /// </summary>
    public string? Translation { get; set; }

    /// <summary>
    /// Gets a Dictionary of cached regexes that are unknown.
    /// </summary>
    protected static Dictionary<string, Regex> CachedRegex { get; } = new();

    /// <summary>
    /// <inheritdoc cref="Build"/>.
    /// </summary>
    /// <param name="builder">The <see cref="TranslationBuilder"/> instance.</param>
    /// <returns><inheritdoc cref="Build"/></returns>
    public static implicit operator string(TranslationBuilder builder) =>
        builder.Build();

    /// <summary>
    /// Creates a compatible placeholder regex.
    /// </summary>
    /// <param name="placeholder">The placeholder.</param>
    /// <returns>The new regex.</returns>
    public static Regex CreateRegex(string placeholder) =>
        new(ToParameterString(placeholder), RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Adds a custom replacer to the <see cref="CustomReplacers"/> dictionary.
    /// </summary>
    /// <param name="toReplace">The regex to replace.</param>
    /// <param name="replacer">The string factory to replace with.</param>
    /// <returns>The <see cref="TranslationBuilder"/> instance.</returns>
    public TranslationBuilder AddCustomReplacer(Regex toReplace, Func<string> replacer)
    {
        CustomReplacers.Add(toReplace, replacer);

        return this;
    }

    /// <summary>
    /// <inheritdoc cref="AddCustomReplacer(System.Text.RegularExpressions.Regex,System.Func{string})"/>
    /// </summary>
    /// <param name="toReplace">The text to replace.</param>
    /// <param name="replacer">The string factory to replace with.</param>
    /// <returns>The <see cref="TranslationBuilder"/> instance.</returns>
    public TranslationBuilder AddCustomReplacer(string toReplace, Func<string> replacer) =>
        AddCustomReplacer(CreateRegex(toReplace), replacer);

    /// <summary>
    /// <inheritdoc cref="AddCustomReplacer(System.Text.RegularExpressions.Regex,System.Func{string})"/>
    /// </summary>
    /// <param name="toReplace">The text to replace.</param>
    /// <param name="replacer">The text to replace with.</param>
    /// <returns>The <see cref="TranslationBuilder"/> instance.</returns>
    public TranslationBuilder AddCustomReplacer(string toReplace, string replacer) =>
        AddCustomReplacer(toReplace, () => replacer);

    /// <summary>
    /// Builds this <see cref="TranslationBuilder"/> instance.
    /// </summary>
    /// <param name="translation">The translation to build from, isn't needed if <see cref="Translation"/> is defined.</param>
    /// <returns>The translation built.</returns>
    public string Build(string? translation = null)
    {
        translation ??= Translation;

        if (string.IsNullOrEmpty(translation))
            throw new ArgumentNullException($"{nameof(TranslationBuilder)} failed to build because of no valid translation.");

        string returnTranslation = translation!;

        foreach (KeyValuePair<Regex, Func<string>> replacer in CustomReplacers)
        {
            returnTranslation = replacer.Key.CheckReplace(returnTranslation, () => GetReplacer(replacer.Value));
        }

        foreach (KeyValuePair<Regex, Func<string>> replacer in StaticReplacers)
        {
            returnTranslation = replacer.Key.CheckReplace(returnTranslation, () => GetReplacer(replacer.Value));
        }

        long unix = new DateTimeOffset(Time).ToUnixTimeSeconds();

        foreach (KeyValuePair<Regex, Func<long, string>> replacer in TimeReplacers)
        {
            returnTranslation = replacer.Key.CheckReplace(
                returnTranslation,
                Replacement);
            continue;

            string Replacement() => GetReplacer(() => replacer.Value(unix));
        }

        return returnTranslation;
    }

    private static string ToParameterString(string str) => "{" + str.Replace(" ", string.Empty) + "}";

#pragma warning disable SA1118
    private static string GetRemainingDecontaminationTime() => Mathf.Min(
            0,
            (float)(DecontaminationController.Singleton
                .DecontaminationPhases[^1]
                .TimeTrigger - DecontaminationController.GetServerTime))
        .ToString(CultureInfo.InvariantCulture);
#pragma warning restore SA1118

    private static TimeSpan TimeSince(long time) =>
        Round.Duration - (DateTimeOffset.Now - DateTimeOffset.FromUnixTimeSeconds(time));

    protected static string GetReplacer(Func<string> func)
    {
        try
        {
            string res = func();

            return string.IsNullOrEmpty(res) ? "Unknown" : res;
        }
        catch (NullReferenceException)
        {
            return "Unknown";
        }
        catch (IndexOutOfRangeException)
        {
            return "Unknown";
        }
    }
}