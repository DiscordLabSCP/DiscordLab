namespace DiscordLab.Bot.API.Extensions
{
    /// <summary>
    /// Contains extension methods for bitwise operations.
    /// </summary>
    public static class BitwiseExtensions
    {
        /// <summary>
        /// Get flags from a <see cref="Enum"/>.
        /// </summary>
        /// <param name="flags">The flags.</param>
        /// <typeparam name="T">The <see cref="Enum"/>.</typeparam>
        /// <returns>The flags that are active.</returns>
        public static IEnumerable<T> GetFlags<T>(this T flags)
            where T : Enum => Enum.GetValues(typeof(T)).Cast<T>().Where(x => flags.HasFlag(x));
    }
}