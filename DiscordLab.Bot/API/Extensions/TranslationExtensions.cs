using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using LabApi.Features.Wrappers;
using PlayerRoles;

namespace DiscordLab.Bot.API.Extensions
{
    public static class TranslationExtensions
    {
        // ReSharper disable once MemberCanBePrivate.Global
        public static long CurrentUnix => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        
        public static bool InProgress => Round.IsRoundStarted && !Round.IsRoundEnded;

        public static PlayerRoleBase GetRoleBase(this RoleTypeId roleTypeId) =>
            PlayerRoleLoader.TryGetRoleTemplate(roleTypeId, out PlayerRoleBase role) ? role : null!;
        
        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        // ReSharper disable once UseCollectionExpression
        public static List<Tuple<string, Func<string>>> StaticReplacers = new () 
        {
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
            
            // Round Replacers
            new("elapsedtime", () => Round.Duration.ToString()),
            new("elapsedtimerelative", () => $"<t:{CurrentUnix-Round.Duration.TotalSeconds}:R>"),
            new("roundstart", () => $"<t:{CurrentUnix-Round.Duration.TotalSeconds}:T>"),
            new("escapedscientistscount", () => Round.EscapedScientists.ToString()),
            new("inprogress", () => InProgress.ToString()),
            new("isended", () => Round.IsRoundEnded.ToString()),
            new("isstarted", () => Round.IsRoundStarted.ToString()),
            new("islocked", () => Round.IsLocked.ToString()),
            new("changedintozombiescount", () => Round.ChangedIntoZombies.ToString()),
            new("escapeddclassescount", () => Round.EscapedClassD.ToString()),
            new("islobbylocked", () => Round.IsLobbyLocked.ToString()),
            new("alivescpcount", () => Round.SurvivingSCPs.ToString()),
            
            // Server Replacers
            new("maxplayers", () => Server.MaxPlayers.ToString()),
            new("name", () => ServerConsole._serverName),
            new("nameparsed", () =>
            {
                const string tagRemoveRegex = @"<[^>]+>";
                const string uselessTextRemove = @"<color=#00000000>(.*?)<\/color>";

                string result = Regex.Replace(ServerConsole._serverName, uselessTextRemove, string.Empty);
                result = Regex.Replace(result, tagRemoveRegex, string.Empty);

                return result;
            }),
            new("port", () => Server.Port.ToString()),
            new("ip", () => Server.IpAddress),
            new("playercount", () => Server.PlayerCount.ToString()),
            new("playercountnonpcs", () => Player.List.Count(p => p.IsPlayer).ToString()),
            new("tps", () => Server.TPS.ToString(CultureInfo.CurrentCulture)),
        };

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        // ReSharper disable once UseCollectionExpression
        public static List<Tuple<string, Func<Player, string>>> PlayerReplacers = new () 
        {
            new("nickname", player => player.Nickname.Replace("@", "\\@")),
            new("id", player => player.UserId),
            new("ip", player => player.IpAddress),
            new("userid", player => player.PlayerId.ToString()),
            new("role", player => player.Role.GetRoleBase().RoleName),
            new("roletype", player => player.Role.ToString()),
            new("team", player => player.Role.GetRoleBase().Team.ToString()),
            new("health", player => player.Health.ToString(CultureInfo.CurrentCulture)),
            new("maxhealth", player => player.MaxHealth.ToString(CultureInfo.CurrentCulture)),
            new("group", player => player.GroupName),
            new("badge", player => player.UserGroup?.BadgeText ?? "Unknown"),
            new("badgecolor", player => player.UserGroup?.BadgeColor ?? "Unknown")
        };
        
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
            builder.Replace($"{{{prefix}}}", player.Nickname);
            foreach ((string placeholder, Func<Player, string> replaceWith) in PlayerReplacers)
            {
                string replacement;
                try
                {
                    replacement = replaceWith(player);
                }
                catch (NullReferenceException)
                {
                    replacement = "Unknown";
                }
                if(string.IsNullOrEmpty(replacement)) replacement = "Unknown";
                builder.Replace($"{{{prefix}{placeholder}}}", replacement);
            }

            return builder.ToString();
        }
    }
}