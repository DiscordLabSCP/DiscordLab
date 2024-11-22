using Exiled.API.Interfaces;

namespace DiscordLab.Moderation
{
    public class Translation : ITranslation
    {
        public string BanCommandName { get; set; } = "ban";
        public string BanCommandDescription { get; set; } = "Bans a player from the server.";
        public string BanCommandUserOptionName { get; set; } = "user";
        public string BanCommandUserOptionDescription { get; set; } = "The player to ban.";
        public string BanCommandDurationOptionName { get; set; } = "duration";
        public string BanCommandDurationOptionDescription { get; set; } = "The duration of the ban (in minutes).";
        public string BanCommandReasonOptionName { get; set; } = "reason";
        public string BanCommandReasonOptionDescription { get; set; } = "The reason for the ban.";
        public string BanCommandSuccess { get; set; } = "Successfully banned `{player}`.";
        public string UnbanCommandName { get; set; } = "unban";
        public string UnbanCommandDescription { get; set; } = "Unbans a player from the server.";
        public string UnbanCommandUserOptionName { get; set; } = "user";
        public string UnbanCommandUserOptionDescription { get; set; } = "The player to unban.";
        public string UnbanCommandSuccess { get; set; } = "Successfully unbanned `{player}`.";
        public string FailedExecuteCommand { get; set; } = "Failed to execute the command. Here is the reason the server gave back: \n```{reason}```";
    }
}