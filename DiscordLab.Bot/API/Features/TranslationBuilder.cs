namespace DiscordLab.Bot.API.Features
{
    using System.Globalization;
    using System.Text.RegularExpressions;
    using Discord;
    using LabApi.Features.Extensions;
    using LabApi.Features.Wrappers;
    using PlayerRoles;

    /// <summary>
    /// Allows you to create translations with placeholders being replaced.
    /// </summary>
    public class TranslationBuilder
    {
        private static readonly Regex TagRemoveRegex = new("<[^>]+>", RegexOptions.Compiled);

        private static readonly Regex UselessTextRemoveRegex = new(@"<color=#00000000>(.*?)<\/color>", RegexOptions.Compiled);

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

#pragma warning disable SA1401 // FieldsMustBePrivate
        /// <summary>
        /// Gets the dictionary of replacers that have no argument.
        /// </summary>
        public static Dictionary<string, Func<string>> StaticReplacers { get; } = new()
        {
            // Map Replacers
            ["seed"] = () => Map.Seed.ToString(),

            // Round Replacers
            ["killcount"] = () => Round.TotalDeaths.ToString(),
            ["elapsedtime"] = () => Round.Duration.ToString(),
            ["escapedscientistscount"] = () => Round.EscapedScientists.ToString(),
            ["inprogress"] = () => Round.IsRoundInProgress.ToString(),
            ["isended"] = () => Round.IsRoundEnded.ToString(),
            ["isstarted"] = () => Round.IsRoundStarted.ToString(),
            ["islocked"] = () => Round.IsLocked.ToString(),
            ["changedintozombiescount"] = () => Round.ChangedIntoZombies.ToString(),
            ["escapeddclassescount"] = () => Round.EscapedClassD.ToString(),
            ["islobbylocked"] = () => Round.IsLobbyLocked.ToString(),
            ["scpkillcount"] = () => Round.KilledBySCPs.ToString(),
            ["alivescpcount"] = () => Round.SurvivingSCPs.ToString(),

            // Server Replacers
            ["maxplayers"] = () => Server.MaxPlayers.ToString(),
            ["name"] = () => Server.ServerListName,
            ["nameparsed"] = () =>
            {
                string result = UselessTextRemoveRegex.Replace(Server.ServerListName, string.Empty);
                result = TagRemoveRegex.Replace(result, string.Empty);

                return result;
            },
            ["port"] = () => Server.Port.ToString(),
            ["ip"] = () => Server.IpAddress,
            ["playercount"] = () => Server.PlayerCount.ToString(),
            ["playercountnonpcs"] = () => Player.ReadyList.Count(p => !p.IsNpc).ToString(),
            ["tps"] = () => Server.Tps.ToString(CultureInfo.CurrentCulture),
        };

        /// <summary>
        /// Gets time based replacers. The <see cref="long"/> type is the unix timestamp. Can be got with <see cref="DateTimeOffset.ToUnixTimeSeconds"/>.
        /// </summary>
        public static Dictionary<string, Func<long, string>> TimeReplacers { get; } = new()
        {
            ["time"] = time => $"<t:{time}>",
            ["timet"] = time => $"<t:{time}:t>",
            ["timetlong"] = time => $"<t:{time}:T>",
            ["timed"] = time => $"<t:{time}:d>",
            ["timedlong"] = time => $"<t:{time}:D>",
            ["timef"] = time => $"<t:{time}:f>",
            ["timeflong"] = time => $"<t:{time}:F>",
            ["timer"] = time => $"<t:{time}:R>",
            ["elapsedtimerelative"] = time => $"<t:{time - Round.Duration.TotalSeconds}:R>",
            ["roundstart"] = time => $"<t:{time - Round.Duration.TotalSeconds}:T>",
        };

        /// <summary>
        /// Gets player based replacements.
        /// </summary>
        public static Dictionary<string, Func<Player, string>> PlayerReplacers { get; } = new()
        {
            ["nickname"] = player => player.Nickname.Replace("@", "\\@"),
            ["id"] = player => player.UserId,
            ["ip"] = player => player.IpAddress,
            ["userid"] = player => player.PlayerId.ToString(),
            ["role"] = player => player.Role.GetFullName(),
            ["roletype"] = player => player.Role.ToString(),
            ["team"] = player => player.Team.ToString(),
            ["faction"] = player => player.Team.GetFaction().ToString(),
            ["health"] = player => player.Health.ToString(CultureInfo.CurrentCulture),
            ["maxhealth"] = player => player.MaxHealth.ToString(CultureInfo.CurrentCulture),
            ["group"] = player => player.GroupName,
            ["badgecolor"] = player => player.GroupColor.ToString(),
        };
#pragma warning restore SA1401 // FieldsMustBePrivate

        /// <summary>
        /// Gets or sets a Dictionary of custom replacers. Key is the text to replace and value is the factory to replace with.
        /// </summary>
        public Dictionary<string, Func<string>> CustomReplacers { get; set; } = new();

        /// <summary>
        /// Gets or sets the players that need to be translated for, if any.
        /// </summary>
        public Dictionary<string, Player> Players { get; set; } = new();

        /// <summary>
        /// Gets or sets the time that this translation will use.
        /// </summary>
        public DateTime Time { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets the translation.
        /// </summary>
        public string Translation { get; }

        /// <summary>
        /// Gets or sets the item that will show for each player when the {players} placeholder is used. Defaults to null.
        /// </summary>
        /// <remarks>If you want the {players} placeholder to not work, set this to null.</remarks>
        public string PlayerListItem { get; set; }

        /// <summary>
        /// Gets or sets the separator between items in <see cref="PlayerListItem"/>.
        /// </summary>
        public string PlayerListSeparator { get; set; } = "\n";

        /// <summary>
        /// Gets or sets the player list that will be used for <see cref="PlayerListItem"/>.
        /// </summary>
        public IEnumerable<Player> PlayerList { get; set; }

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
        /// <param name="toReplace">The text to replace.</param>
        /// <param name="replacer">The string factory to replace with.</param>
        /// <returns>The <see cref="TranslationBuilder"/> instance.</returns>
        public TranslationBuilder AddCustomReplacer(string toReplace, Func<string> replacer)
        {
            CustomReplacers.Add(toReplace, replacer);

            return this;
        }

        /// <summary>
        /// Adds a custom replacer to the <see cref="CustomReplacers"/> dictionary.
        /// </summary>
        /// <param name="toReplace">The text to replace.</param>
        /// <param name="replacer">The text to replace with.</param>
        /// <returns>The <see cref="TranslationBuilder"/> instance.</returns>
        public TranslationBuilder AddCustomReplacer(string toReplace, string replacer)
        {
            AddCustomReplacer(toReplace, () => replacer);

            return this;
        }

        /// <summary>
        /// Builds this <see cref="TranslationBuilder"/> instance.
        /// </summary>
        /// <returns>The translation built.</returns>
        public string Build()
        {
            if (PlayerListItem != null)
                SetupPlayerList();

            string returnTranslation = Translation;

            foreach (KeyValuePair<string, Func<string>> replacer in CustomReplacers)
            {
                returnTranslation = Regex.Replace(
                    returnTranslation,
                    ToParameterString(replacer.Key),
                    replacer.Value(),
                    RegexOptions.IgnoreCase);
            }

            foreach (KeyValuePair<string, Func<string>> replacer in StaticReplacers)
            {
                returnTranslation = Regex.Replace(
                    returnTranslation,
                    ToParameterString(replacer.Key),
                    replacer.Value(),
                    RegexOptions.IgnoreCase);
            }

            long unix = new DateTimeOffset(Time).ToUnixTimeSeconds();
            foreach (KeyValuePair<string, Func<long, string>> replacer in TimeReplacers)
            {
                returnTranslation = Regex.Replace(
                    returnTranslation,
                    ToParameterString(replacer.Key),
                    replacer.Value(unix),
                    RegexOptions.IgnoreCase);
            }

            foreach (KeyValuePair<string, Player> player in Players)
            {
                returnTranslation = Regex.Replace(
                    returnTranslation,
                    ToParameterString(player.Key),
                    player.Value.Nickname,
                    RegexOptions.IgnoreCase);

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

                    if (string.IsNullOrEmpty(replacement))
                        replacement = "Unknown";

                    returnTranslation = Regex.Replace(
                        returnTranslation,
                        ToParameterString($"{player.Key}{replacer.Key}"),
                        replacement,
                        RegexOptions.IgnoreCase);
                }
            }

            return returnTranslation;
        }

        private static string ToParameterString(string str) => "{" + str + "}";

        private void SetupPlayerList()
        {
            Player[] readyPlayers = (PlayerList ?? Player.ReadyList).ToArray();

            int length = readyPlayers.Length;

            List<string> playerItems = new(length);
            Dictionary<string, Player> playerDictionary = new(length);

            for (int i = 0; i < length; i++)
            {
                string playerKey = $"player{i}";
                playerItems.Add(PlayerListItem.Replace("{player", "{" + $"{playerKey}"));
                playerDictionary[playerKey] = readyPlayers[i];
            }

            CustomReplacers.Add("players", () => string.Join(PlayerListSeparator, playerItems));

            AddPlayers(playerDictionary);
        }
    }
}