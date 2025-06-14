using System.Globalization;

namespace DiscordLab.Bot.API.Extensions
{
    public static class ColorExtensions
    {
        public static uint GetColor(this string color)
        {
            return uint.Parse(color, NumberStyles.HexNumber);
        }
    }
}