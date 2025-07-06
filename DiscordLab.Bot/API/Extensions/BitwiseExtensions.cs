namespace DiscordLab.Bot.API.Extensions
{
    public static class BitwiseExtensions
    {
        public static IEnumerable<T> GetFlags<T>(this T flags)
            where T : Enum => Enum.GetValues(typeof(T)).Cast<T>().Where(x => flags.HasFlag(x));
    }
}