using Exiled.API.Features;
using JetBrains.Annotations;

namespace DiscordLab.Bot.API.Extensions
{
    public static class TranslationExtensions
    {
        public static string PlayerReplace(this string str, Player main, [CanBeNull] Player secondary)
        {
            string newString = str.LowerReplace("{Player}", main.Nickname)
                .LowerReplace("{PlayerName}", main.Nickname)
                .LowerReplace("{PlayerId}", main.UserId)
                .LowerReplace("{PlayerRAId}", main.Id.ToString())
                .LowerReplace("{PlayerRole}", main.Role.Name)
                .LowerReplace("{PlayerIP}", main.IPAddress)
                .LowerReplace("{PlayerBadge}", main.RankName);

            if (secondary != null)
            {
                newString = newString.Replace("{Secondary}", secondary.Nickname)
                    .Replace("{SecondaryName}", secondary.Nickname)
                    .Replace("{SecondaryId}", secondary.UserId)
                    .Replace("{SecondaryRAId}", secondary.Id.ToString())
                    .Replace("{SecondaryRole}", secondary.Role.Name)
                    .Replace("{SecondaryIP}", secondary.IPAddress)
                    .Replace("{SecondaryBadge}", secondary.RankName);
            }

            return newString;
        }
        
        public static string LowerReplace(this string str, string replace, string value)
        {
            return str.Replace(replace.ToLower(), value);
        }
    }
}