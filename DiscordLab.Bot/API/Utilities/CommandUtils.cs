namespace DiscordLab.Bot.API.Utilities
{
    using LabApi.Features.Wrappers;

    /// <summary>
    /// Utility methods for commands.
    /// </summary>
    public static class CommandUtils
    {
        /// <summary>
        /// Gets a player from an unparsed string id, will check if <see cref="Player.PlayerId"/> or <see cref="Player.UserId"/>.
        /// </summary>
        /// <param name="id">The ID to check.</param>
        /// <returns>The player if found.</returns>
        public static Player GetPlayerFromUnparsed(string id)
        {
            return TryGetPlayerFromUnparsed(id, out Player player) ? player : null;
        }

        /// <summary>
        /// Tries to get a player from an unparsed string id, will check if <see cref="Player.PlayerId"/> or <see cref="Player.UserId"/>.
        /// </summary>
        /// <param name="id">The ID to check.</param>
        /// <param name="player">The player if found.</param>
        /// <returns>Whether the player was found.</returns>
        public static bool TryGetPlayerFromUnparsed(string id, out Player player)
        {
            if (int.TryParse(id, out int intId))
            {
                if (!Player.TryGet(intId, out player))
                {
                    return false;
                }
            }
            else
            {
                if (!Player.TryGet(id, out player))
                {
                    return false;
                }
            }

            return true;
        }
    }
}