using System.Globalization;
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
            new("kills", () => Round.Kills.ToString())
        ];

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static List<Tuple<string, Func<Player, string>>> PlayerReplacers =
        [
            new("nickname", player => player.Nickname),
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
                m.Groups[1].Success ? m.Groups[1].Value.ToLower() : m.Groups[2].Value
            );
        }

        public static string StaticReplace(this string str)
        {
            string ret = str;
            foreach ((string placeholder, Func<string> replaceWith) in StaticReplacers)
            {
                ret = str.Replace($"{{{placeholder}}}", replaceWith());
            }

            return ret;
        }

        public static string PlayerReplace(this string str, string prefix, Player player)
        {
            string ret = str;
            foreach ((string placeholder, Func<Player, string> replaceWith) in PlayerReplacers)
            {
                ret = str.Replace($"{{{prefix}{placeholder}}}", replaceWith(player));
            }

            return ret;
        }
    }
}