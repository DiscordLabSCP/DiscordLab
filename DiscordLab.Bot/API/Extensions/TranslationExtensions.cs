using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Exiled.API.Features;

namespace DiscordLab.Bot.API.Extensions
{
    public static class TranslationExtensions
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public static long CurrentUnix => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static List<Tuple<string, Func<string>>> StaticReplacers =
        [
            // Time Replacers
            new("time", () => $"<t:{CurrentUnix}>"),
            new("timet", () => $"<t:{CurrentUnix}:t>"),
            new("timetlong", () => $"<t:{CurrentUnix}:T>"),
            new("timed", () => $"<t:{CurrentUnix}:d>"),
            new("timedlong", () => $"<t:{CurrentUnix}:D>"),
            new("timef", () => $"<t:{CurrentUnix}:f>"),
            new("timeflong", () => $"<t:{CurrentUnix}:F>"),
            new("timer", () => $"<t:{CurrentUnix}:R>"),
            
            // Map Replacers
            new("seed", () => Map.Seed.ToString()),
            new("decontstate", () => Map.DecontaminationState.ToString()),
            new("isdecont", () => Map.IsLczDecontaminated.ToString()),
            new("isdecontenabled", () => Map.IsDecontaminationEnabled.ToString()),
            
            // Round Replacers
            new("killcount", () => Round.Kills.ToString()),
            new("alivesides", () => string.Join(", ", Round.AliveSides.Select(s => s.ToString()))),
            new("alivesidescount", () => Round.AliveSides.Count().ToString()),
            new("elapsedtime", () => Round.ElapsedTime.ToString()),
            new("elapsedtimerelative", () => $"<t:{CurrentUnix-Round.ElapsedTime.TotalSeconds}:R>"),
            new("roundstart", () => $"<t:{CurrentUnix-Round.ElapsedTime.TotalSeconds}:T>"),
            new("roundcount", () => Round.UptimeRounds.ToString()),
            new("escapedscientistscount", () => Round.EscapedScientists.ToString()),
            new("inprogress", () => Round.InProgress.ToString()),
            new("isended", () => Round.IsEnded.ToString()),
            new("isstarted", () => Round.IsStarted.ToString()),
            new("islocked", () => Round.IsLocked.ToString()),
            new("islobby", () => Round.IsLobby.ToString()),
            new("changedintozombiescount", () => Round.ChangedIntoZombies.ToString()),
            new("escapeddclassescount", () => Round.EscapedDClasses.ToString()),
            new("islobbylocker", () => Round.IsLobbyLocked.ToString()),
            new("scpkillcount", () => Round.KillsByScp.ToString()),
            new("alivescpcount", () => Round.SurvivingSCPs.ToString()),
            
            // Server Replacers
            new("maxplayers", () => Server.MaxPlayerCount.ToString()),
            new("name", () => Server.Name),
            new("nameparsed", () =>
            {
                const string tagRemoveRegex = @"<[^>]+>";
                const string uselessTextRemove = @"<color=#00000000>(.*?)<\/color>";

                string result = Regex.Replace(Server.Name, uselessTextRemove, string.Empty);
                result = Regex.Replace(result, tagRemoveRegex, string.Empty);

                return result;
            }),
            new("port", () => Server.Port.ToString()),
            new("ip", () => Server.IpAddress),
            new("playercount", () => Server.PlayerCount.ToString()),
            new("playercountnonpcs", () => Player.List.Count(p => !p.IsNPC).ToString()),
            new("tps", () => Server.Tps.ToString(CultureInfo.CurrentCulture)),
        ];

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static List<Tuple<string, Func<Player, string>>> PlayerReplacers =
        [
            new("nickname", player => player.Nickname.Replace("@", "\\@")),
            new("id", player => player.UserId),
            new("ip", player => player.IPAddress),
            new("userid", player => player.Id.ToString()),
            new("role", player => player.Role.Name),
            new("roletype", player => player.Role.Type.ToString()),
            new("team", player => player.Role.Team.ToString()),
            new("side", player => player.Role.Side.ToString()),
            new("health", player => player.Health.ToString(CultureInfo.CurrentCulture)),
            new("maxhealth", player => player.MaxHealth.ToString(CultureInfo.CurrentCulture)),
            new("group", player => player.GroupName),
            new("badge", player => player.Group.BadgeText),
            new("badgecolor", player => player.Group.BadgeColor)
        ];
        
        /// <summary>
        /// Makes all parameters lowercase and keeps the rest of the translation in its original state.
        /// </summary>
        /// <param name="str">The translation</param>
        /// <returns>The translation with lowercase params</returns>
        public static string LowercaseParams(this string str)
        {
            const string pattern = @"\{(.*?)\}|(.)";

            return Regex.Replace(
                str, 
                pattern, 
                m => 
                m.Groups[1].Success ? $"{{{m.Groups[1].Value.ToLower()}}}" : m.Groups[2].Value
            );
        }

        public static string StaticReplace(this string str)
        {
            StringBuilder builder = new(str);
            foreach ((string placeholder, Func<string> replaceWith) in StaticReplacers)
            {
                builder.Replace($"{{{placeholder}}}", replaceWith());
            }

            return builder.ToString();
        }

        public static string PlayerReplace(this string str, string prefix, Player player)
        {
            StringBuilder builder = new(str);
            foreach ((string placeholder, Func<Player, string> replaceWith) in PlayerReplacers)
            {
                builder.Replace($"{{{prefix}{placeholder}}}", replaceWith(player));
            }

            return builder.ToString();
        }
    }
}