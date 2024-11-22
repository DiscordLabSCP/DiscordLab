using System.ComponentModel;
using Exiled.API.Interfaces;

namespace DiscordLab.XPSystem
{
    public class Translation : ITranslation
    {
        public string LevelUp { get; set; } = "Player {playername} ({playerid}) has leveled up to level {level}!";

        [Description("The command name for getting the level of a player. Can not have spaces or special characters.")]
        public string CommandName { get; set; } = "getlevel";

        [Description("The description of the command.")]
        public string CommandDescription { get; set; } = "Get the level of a player.";

        [Description("The name of the option for the command.")]
        public string CommandOptionName { get; set; } = "player";

        [Description("The description of the option for the command.")]
        public string CommandOptionDescription { get; set; } = "The player to get the level of.";

        [Description("The message to send when the user fails to get a user.")]
        public string FailToGetUser { get; set; } = "Failed to get user.";

        [Description("The title of the embed.")]
        public string EmbedTitle { get; set; } = "Player Level";

        [Description("The start of the description of the embed.")]
        public string EmbedDescription { get; set; } =
            "The level of the player is {level}.\nThey have {currentxp} out of {neededxp} XP.";

        [Description("The footer of the embed.")]
        public string EmbedFooter { get; set; } = "Requested user: {user} ({userid})";
    }
}