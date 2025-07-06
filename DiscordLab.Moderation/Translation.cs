using System.ComponentModel;
using Discord;

namespace DiscordLab.Moderation
{
    public class Translation
    {
        public SlashCommandBuilder MuteCommand { get; set; } = new()
        {
            Name = "mute",
            Description = "Mute a player on the server",
            DefaultMemberPermissions = GuildPermission.ModerateMembers,
            Options =
            [
                new()
                {
                    Name = "user",
                    Description = "The user to mute"
                },
                new()
                {
                    Name = "duration",
                    Description = "The duration to mute the user for"
                }
            ]
        };

        public SlashCommandBuilder UnmuteCommand { get; set; } = new()
        {
            Name = "unmute",
            Description = "Unmute a player on the server",
            DefaultMemberPermissions = GuildPermission.ModerateMembers,
            Options =
            [
                new()
                {
                    Name = "user",
                    Description = "The user to unmute"
                }
            ]
        };

        public SlashCommandBuilder BanCommand { get; set; } = new()
        {
            Name = "ban",
            Description = "Ban a player on the server",
            DefaultMemberPermissions = GuildPermission.ModerateMembers,
            Options =
            [
                new()
                {
                    Name = "user",
                    Description = "The user to ban"
                },
                new()
                {
                    Name = "duration",
                    Description = "The duration to ban the user for"
                },
                new()
                {
                    Name = "reason",
                    Description = "The reason to ban the user"
                }
            ]
        };
        
        public SlashCommandBuilder UnbanCommand { get; set; } = new()
        {
            Name = "unban",
            Description = "Unban a player on the server",
            DefaultMemberPermissions = GuildPermission.ModerateMembers,
            Options =
            [
                new()
                {
                    Name = "user",
                    Description = "The user to unban"
                }
            ]
        };

        public string InvalidUser { get; set; } = "Please provide a valid user to use this command on.";
        
        public string TempMuteSuccess { get; set; } = "Player {player} has been temporarily muted for {duration}. They will get unmuted at {timef}";

        public string UnmuteSuccess { get; set; } = "Player {player} has been successfully unmuted.";
        
        public string PermMuteSuccess { get; set; } = "Player {player} has been muted.";

        public string BanFailure { get; set; } = "Failed to ban {userid}. Please make sure the data is valid and try again...";

        public string BanSuccess { get; set; } = "Successfully banned {userid} for {reason}. They will get unbanned in {timer}";

        public string UnbanSuccess { get; set; } = "Player {userid} has been unbanned.";

        public string PermMuteLog { get; set; } = "Player {target} has been muted by {player}.";

        public string TempMuteLog { get; set; } =
            "Player {target} has been muted by {player} for {timef}, they will be unmuted in {timer}";

        public string UnmuteLog { get; set; } = "Player {target} has been unmuted by {player}.";

        [Description(
            "Every field value accepts placeholders, even if you add more. player in this case is the issuer.")]
        public EmbedBuilder BanLogEmbed { get; set; } = new()
        {
            Title = "Ban Log",
            Description = "A user has been banned",
            Fields =
            [
                new()
                {
                    Name = "Player",
                    Value = "{userid}"
                },
                new()
                {
                    Name = "Issuer",
                    Value = "{player}"
                },
                new()
                {
                    Name = "Duration",
                    Value = "{timer} ({timef})"
                }
            ]
        };

        [Description("Normal player things may not work here, but playerid always will, unless somehow banned by something without an ID.")]
        public string UnbanLog { get; set; } = "Player {username} ({userid}) has been unbanned by {playerid}";
    }
}