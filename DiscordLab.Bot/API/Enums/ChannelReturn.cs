namespace DiscordLab.Bot.API.Enums
{
    /// <summary>
    /// Used to determine if a channel was found or not, and if not, the reason why it wasn't found.
    /// </summary>
    public enum ChannelReturn
    {
        /// <summary>
        /// The guild and channel were found.
        /// </summary>
        Found = 0,
        /// <summary>
        /// Couldn't find the guild with the specified ID.
        /// </summary>
        InvalidGuild = 1,
        /// <summary>
        /// The guild ID was 0.
        /// </summary>
        NoGuild = 2,
        /// <summary>
        /// Couldn't find the channel with the specified ID.
        /// </summary>
        InvalidChannel = 3,
        /// <summary>
        /// The channel ID was 0.
        /// </summary>
        NoChannel = 4,
        /// <summary>
        /// The channel type was not the requested type.
        /// </summary>
        InvalidType = 5
    }
}