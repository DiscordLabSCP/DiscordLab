namespace DiscordLab.Bot.API.Enums
{
    /// <summary>
    /// Used to determine if a guild was found or not, and if not, the reason why it wasn't found.
    /// </summary>
    public enum GuildReturn
    {
        /// <summary>
        /// The guild was found.
        /// </summary>
        Found = 0,
        /// <summary>
        /// The guild ID was invalid.
        /// </summary>
        InvalidGuild = 1,
        /// <summary>
        /// The guild ID was 0.
        /// </summary>
        NoGuild = 2
    }
}