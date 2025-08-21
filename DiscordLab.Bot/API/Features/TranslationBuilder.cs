// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
namespace DiscordLab.Bot.API.Features;

using System.Globalization;
using System.Text.RegularExpressions;
using Discord;
using LabApi.Features.Wrappers;
using LightContainmentZoneDecontamination;
using Mirror.LiteNetLib4Mirror;
using PlayerRoles;
using RoundRestarting;
using UnityEngine;

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
    /// Initializes a new instance of the <see cref="TranslationBuilder"/> class with a person added.
    /// </summary>
    /// <param name="playerPrefix">The player prefix.</param>
    /// <param name="player">The player to use for the prefix.</param>
    public TranslationBuilder(string playerPrefix, Player player)
    {
        AddPlayer(playerPrefix, player);
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
    /// Initializes a new instance of the <see cref="TranslationBuilder"/> class with a person added.
    /// </summary>
    /// <param name="translation">The translation to modify.</param>
    /// <param name="playerPrefix">The player prefix.</param>
    /// <param name="player">The player to use for the prefix.</param>
    public TranslationBuilder(string translation, string playerPrefix, Player player)
    {
        Translation = translation;
        AddPlayer(playerPrefix, player);
    }

    /// <summary>
    /// Gets the dictionary of replacers that have no argument.
    /// </summary>
    public static Dictionary<Regex, Func<string>> StaticReplacers { get; } = new()
    {
        // Map Replacers
        [CreateRegex("seed")] = () => Map.Seed.ToString(),
        [CreateRegex("isdecont")] = () => Decontamination.IsDecontaminating.ToString(),
        [CreateRegex("remainingdeconttime")] = GetRemainingDecontaminationTime,
        [CreateRegex("isdecontenabled")] = () => (Decontamination.Status == DecontaminationController.DecontaminationStatus.None).ToString(),

        // Round Replacers
        [CreateRegex("killcount")] = () => Round.TotalDeaths.ToString(),
        [CreateRegex("elapsedtime")] = () => Round.Duration.ToString(),
        [CreateRegex("escapedscientistscount")] = () => Round.EscapedScientists.ToString(),
        [CreateRegex("inprogress")] = () => Round.IsRoundInProgress.ToString(),
        [CreateRegex("isended")] = () => Round.IsRoundEnded.ToString(),
        [CreateRegex("isstarted")] = () => Round.IsRoundStarted.ToString(),
        [CreateRegex("islocked")] = () => Round.IsLocked.ToString(),
        [CreateRegex("changedintozombiescount")] = () => Round.ChangedIntoZombies.ToString(),
        [CreateRegex("escapeddclassescount")] = () => Round.EscapedClassD.ToString(),
        [CreateRegex("islobbylocked")] = () => Round.IsLobbyLocked.ToString(),
        [CreateRegex("scpkillcount")] = () => Round.KilledBySCPs.ToString(),
        [CreateRegex("alivescpcount")] = () => Round.SurvivingSCPs.ToString(),
        [CreateRegex("roundcount")] = () => RoundRestart.UptimeRounds.ToString(),

        // Server Replacers
        [CreateRegex("maxplayers")] = () => Server.MaxPlayers.ToString(),
        [CreateRegex("name")] = () => Server.ServerListName,
        [CreateRegex("nameparsed")] = () =>
        {
            string result = UselessTextRemoveRegex.Replace(Server.ServerListName, string.Empty);
            result = TagRemoveRegex.Replace(result, string.Empty);

            return result;
        },
        [CreateRegex("port")] = () => Server.Port.ToString(),
        [CreateRegex("ip")] = () => Server.IpAddress,
        [CreateRegex("playercount")] = () => Server.PlayerCount.ToString(),
        [CreateRegex("playercountnonpcs")] = () => Player.ReadyList.Count(p => !p.IsNpc).ToString(),
        [CreateRegex("tps")] = () => Server.Tps.ToString(CultureInfo.CurrentCulture),
        [CreateRegex("version")] = () => GameCore.Version.VersionString,
        [CreateRegex("isbeta")] = () => (GameCore.Version.PublicBeta || GameCore.Version.PublicBeta).ToString(),
        [CreateRegex("isfriendlyfire")] = () => Server.FriendlyFire.ToString(),
    };

    /// <summary>
    /// Gets time based replacers. The <see cref="long"/> type is the unix timestamp. Can be got with <see cref="DateTimeOffset.ToUnixTimeSeconds"/>.
    /// </summary>
    public static Dictionary<Regex, Func<long, string>> TimeReplacers { get; } = new()
    {
        [CreateRegex("time")] = time => $"<t:{time}>",
        [CreateRegex("timet")] = time => $"<t:{time}:t>",
        [CreateRegex("timetlong")] = time => $"<t:{time}:T>",
        [CreateRegex("timed")] = time => $"<t:{time}:d>",
        [CreateRegex("timedlong")] = time => $"<t:{time}:D>",
        [CreateRegex("timef")] = time => $"<t:{time}:f>",
        [CreateRegex("timeflong")] = time => $"<t:{time}:F>",
        [CreateRegex("timer")] = time => $"<t:{time}:R>",
        [CreateRegex("elapsedtimerelative")] = time => $"<t:{time - Round.Duration.TotalSeconds}:R>",
        [CreateRegex("roundstart")] = time => $"<t:{time - Round.Duration.TotalSeconds}:T>",
        [CreateRegex("secondssince")] = time => TimeSince(time).Seconds.ToString(CultureInfo.InvariantCulture),
        [CreateRegex("minutessince")] = time => TimeSince(time).Minutes.ToString(CultureInfo.InvariantCulture),
    };

    /// <summary>
    /// Gets player based replacements.
    /// </summary>
    public static Dictionary<string, Func<Player, string>> PlayerReplacers { get; } = new()
    {
        ["name"] = player =>
            player.Nickname.Replace("@everyone", "@\u200beveryone").Replace("@here", "@\u200bhere").Trim(),
        ["nickname"] = player =>
            player.Nickname.Replace("@everyone", "@\u200beveryone").Replace("@here", "@\u200bhere").Trim(),
        ["displayname"] = player => player.DisplayName,
        ["id"] = player => player.UserId,
        ["ip"] = player => player.IpAddress,
        ["userid"] = player => player.PlayerId.ToString(),
        ["role"] = player => player.RoleBase.RoleName,
        ["roletype"] = player => player.Role.ToString(),
        ["team"] = player => player.Team.ToString(),
        ["faction"] = player => player.Team.GetFaction().ToString(),
        ["health"] = player => player.Health.ToString(CultureInfo.CurrentCulture),
        ["maxhealth"] = player => player.MaxHealth.ToString(CultureInfo.CurrentCulture),
        ["group"] = player => player.GroupName,
        ["badgecolor"] = player => player.GroupColor.ToString(),
        ["hasdnt"] = player => player.DoNotTrack.ToString(),
        ["hasra"] = player => player.RemoteAdminAccess.ToString(),
        ["isnorthwood"] = player => player.IsNorthwoodStaff.ToString(),
        ["room"] = player => player.Room?.ToString() ?? "None",
        ["zone"] = player => player.Zone.ToString(),
        ["position"] = player => player.Position.ToString(),
        ["ping"] = player => LiteNetLib4MirrorServer.GetPing(player.Connection.connectionId).ToString(),
        ["isglobalmod"] = player => player.IsGlobalModerator.ToString(),
        ["permissiongroup"] = player => player.PermissionsGroupName ?? "None",
    };

    /// <summary>
    /// Gets or sets a Dictionary of custom replacers. Key is the text to replace and value is the factory to replace with.
    /// </summary>
    public Dictionary<Regex, Func<string>> CustomReplacers { get; set; } = new();

    /// <summary>
    /// Gets or sets the players that need to be translated for, if any.
    /// </summary>
    public Dictionary<string, Player> Players { get; set; } = new();

    /// <summary>
    /// Gets or sets the time that this translation will use.
    /// </summary>
    public DateTime Time { get; set; } = DateTime.Now;

    /// <summary>
    /// Gets or sets the translation.
    /// </summary>
    public string? Translation { get; set; }

    /// <summary>
    /// Gets or sets the item that will show for each player when the {players} placeholder is used. Defaults to null.
    /// </summary>
    /// <remarks>If you want the {players} placeholder to not work, set this to null.</remarks>
    public string? PlayerListItem { get; set; }

    /// <summary>
    /// Gets or sets the separator between items in <see cref="PlayerListItem"/>.
    /// </summary>
    public string PlayerListSeparator { get; set; } = "\n";

    /// <summary>
    /// Gets or sets the player list that will be used for <see cref="PlayerListItem"/>.
    /// </summary>
    public IEnumerable<Player>? PlayerList { get; set; }

    /// <summary>
    /// Gets a Dictionary of cached regexes that are unknown.
    /// </summary>
    private static Dictionary<string, Regex> CachedRegex { get; } = new();

    /// <summary>
    /// <inheritdoc cref="Build"/>.
    /// </summary>
    /// <param name="builder">The <see cref="TranslationBuilder"/> instance.</param>
    /// <returns><inheritdoc cref="Build"/></returns>
    public static implicit operator string(TranslationBuilder builder) =>
        builder.Build();

    /// <summary>
    /// <inheritdoc cref="Build"/>.
    /// </summary>
    /// <param name="builder">The <see cref="TranslationBuilder"/> instance.</param>
    /// <returns><inheritdoc cref="Build"/></returns>
    public static implicit operator Optional<string>(TranslationBuilder builder) =>
        builder.Build();

    /// <summary>
    /// Creates a compatible placeholder regex.
    /// </summary>
    /// <param name="placeholder">The placeholder.</param>
    /// <returns>The new regex.</returns>
    public static Regex CreateRegex(string placeholder) => new(ToParameterString(placeholder), RegexOptions.IgnoreCase | RegexOptions.Compiled);

    /// <summary>
    /// Adds multiple players to the <see cref="Players"/> list.
    /// </summary>
    /// <param name="players">The players to add.</param>
    /// <returns>The <see cref="TranslationBuilder"/> instance.</returns>
    public TranslationBuilder AddPlayers(Dictionary<string, Player> players)
    {
        foreach (KeyValuePair<string, Player> pair in players)
        {
            Players.Add(pair.Key, pair.Value);
        }

        return this;
    }

    /// <summary>
    /// Adds a player to the <see cref="Players"/> list.
    /// </summary>
    /// <param name="prefix">The prefix for the player.</param>
    /// <param name="player">The <see cref="Player"/> to add.</param>
    /// <returns>The <see cref="TranslationBuilder"/> instance.</returns>
    public TranslationBuilder AddPlayer(string prefix, Player player)
    {
        Players.Add(prefix, player);

        return this;
    }

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

        if (PlayerListItem != null && CustomReplacers.All(replacer => replacer.Key.ToString() != "{players}"))
            SetupPlayerList();

        string returnTranslation = translation!;

        foreach (KeyValuePair<Regex, Func<string>> replacer in CustomReplacers)
        {
            returnTranslation = replacer.Key.Replace(returnTranslation, replacer.Value());
        }

        foreach (KeyValuePair<Regex, Func<string>> replacer in StaticReplacers)
        {
            returnTranslation = replacer.Key.Replace(returnTranslation, replacer.Value());
        }

        long unix = new DateTimeOffset(Time).ToUnixTimeSeconds();

        foreach (KeyValuePair<Regex, Func<long, string>> replacer in TimeReplacers)
        {
            returnTranslation = replacer.Key.Replace(
                returnTranslation,
                replacer.Value(unix));
        }

        foreach (KeyValuePair<string, Player> player in Players)
        {
            if (player.Value is not { IsReady: true })
                continue;

            Regex baseRegex = CachedRegex.GetOrAdd(player.Key, () => CreateRegex(player.Key));

            returnTranslation = baseRegex.Replace(returnTranslation, player.Value.Nickname);

            foreach (KeyValuePair<string, Func<Player, string>> replacer in PlayerReplacers)
            {
                string replacement;

                try
                {
                    replacement = replacer.Value(player.Value);
                }
                catch (NullReferenceException)
                {
                    replacement = "Unknown";
                }
                catch (IndexOutOfRangeException)
                {
                    replacement = "Unknown";
                }

                if (string.IsNullOrEmpty(replacement))
                    replacement = "Unknown";

                Regex regex = CachedRegex.GetOrAdd(
                    $"{player.Key}{replacer.Key}",
                    () => CreateRegex($"{player.Key}{replacer.Key}"));

                returnTranslation = regex.Replace(returnTranslation, replacement);
            }
        }

        return returnTranslation;
    }

    private static string ToParameterString(string str) => "{" + str + "}";

#pragma warning disable SA1118
    private static string GetRemainingDecontaminationTime() => Mathf.Min(
            0,
            (float)(DecontaminationController.Singleton
                .DecontaminationPhases[DecontaminationController.Singleton.DecontaminationPhases.Length - 1]
                .TimeTrigger - DecontaminationController.GetServerTime))
        .ToString(CultureInfo.InvariantCulture);
#pragma warning restore SA1118

    private static TimeSpan TimeSince(long time) =>
        Round.Duration - (DateTimeOffset.Now - DateTimeOffset.FromUnixTimeSeconds(time));

    private void SetupPlayerList()
    {
        if (string.IsNullOrEmpty(PlayerListItem))
            throw new ArgumentException($"Invalid {nameof(PlayerListItem)} provided, it was either null or empty.");

        Player[] readyPlayers = (PlayerList ?? Player.ReadyList).ToArray();

        int length = readyPlayers.Length;

        List<string> playerItems = new(length);
        Dictionary<string, Player> playerDictionary = new(length);

        for (int i = 0; i < length; i++)
        {
            string playerKey = $"player{i}";
            playerItems.Add(PlayerListItem!.Replace("{player", "{" + $"{playerKey}"));
            playerDictionary[playerKey] = readyPlayers[i];
        }

        AddCustomReplacer("players", string.Join(PlayerListSeparator, playerItems));

        AddPlayers(playerDictionary);
    }
}