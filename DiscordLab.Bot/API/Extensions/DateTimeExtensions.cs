using JetBrains.Annotations;

namespace DiscordLab.Bot.API.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToDiscordUnixTimestamp(this DateTime dateTime, string suffix = "")
        {
            if (suffix != "") return $"<t:{dateTime.ToUnixTimestamp()}:{suffix}>";
            return $"<t:{dateTime.ToUnixTimestamp()}>";
        }

        public static long ToUnixTimestamp(this DateTime dateTime)
        {
            return new DateTimeOffset(dateTime).ToUnixTimeSeconds();
        }
    }
}