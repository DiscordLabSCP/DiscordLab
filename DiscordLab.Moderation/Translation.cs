using System.ComponentModel;
using DiscordLab.Core;
using DiscordLab.Core.API.Commands;
using DiscordLab.Core.API.Embed;

namespace DiscordLab.Moderation;

public class Translation
{
    // String properties for command and option names

    public CommandBuilder MuteCommand = new()
    {
        Name = "mute",
        Description = "Mute a player on the server",
        Options =
        [
            new GameUserCommandOptionBuilder
            {
                Name = "user",
                Description = "The user to mute",
                IsRequired = true
            },
            new()
            {
                Type = CommandOptionType.String,
                Name = "duration",
                Description = "The duration to mute the user for"
            }
        ],
        DefaultPermission = DefaultCommandPermissions.Moderators
    };

    public CommandBuilder UnmuteCommand = new()
    {
        Name = "unmute",
        Description = "Unmute a player on the server",
        Options =
        [
            new GameUserCommandOptionBuilder
            {
                Name = "user",
                Description = "The user to unmute",
                IsRequired = true
            }
        ],
        DefaultPermission = DefaultCommandPermissions.Moderators
    };

    public CommandBuilder BanCommand = new()
    {
        Name = "ban",
        Description = "Ban a player on the server",
        Options =
        [
            new GameUserCommandOptionBuilder
            {
                Name = "user",
                Description = "The user to ban",
                IsRequired = true
            },
            new()
            {
                Type = CommandOptionType.String,
                Name = "duration",
                Description = "The duration to ban the user for",
                IsRequired = true
            },
            new()
            {
                Type = CommandOptionType.String,
                Name = "reason",
                Description = "The reason to ban the user",
                IsRequired = true
            }
        ],
        DefaultPermission = DefaultCommandPermissions.Moderators
    };

    public CommandBuilder UnbanCommand = new()
    {
        Name = "unban",
        Description = "Unban a player on the server",
        Options =
        [
            new GameUserCommandOptionBuilder
            {
                Name = "user",
                Description = "The user to unban",
                IsRequired = true
            }
        ],
        DefaultPermission = DefaultCommandPermissions.Moderators
    };

    public string InvalidUser { get; set; } = "Please provide a valid user to use this command on.";

    public string TempMuteSuccess { get; set; } =
        "Player {player} has been temporarily muted for {duration}. They will get unmuted at {timef} ({timer})";

    public string UnmuteSuccess { get; set; } = "Player {player} has been successfully unmuted.";

    public string PermMuteSuccess { get; set; } = "Player {player} has been muted.";

    public string BanFailure { get; set; } =
        "Failed to ban `{userid}`. Please make sure the data is valid and try again...";

    public string BanSuccess { get; set; } =
        "Successfully banned `{userid}` for `{reason}`. They will get unbanned at {timef} ({timer})";

    public string UnbanSuccess { get; set; } = "Player `{userid}` has been unbanned.";

    public MessageContent PermMuteLog { get; set; } = "Player {target} has been muted by {player}.";

    public MessageContent TempMuteLog { get; set; } =
        "Player {target} has been muted by {player}. They will get unbanned at {timef} ({timer})";

    public MessageContent UnmuteLog { get; set; } = "Player {target} has been unmuted by {player}.";

    [Description(
        "Every field value accepts placeholders, even if you add more. player in this case is the issuer.")]
    public MessageContent BanLogEmbed { get; set; } = new EmbedBuilder
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
            },
            new()
            {
                Name = "Reason",
                Value = "{reason}"
            }
        ]
    };

    [Description(
        "Normal player things may not work here, but playerid always will, unless somehow banned by something without an ID.")]
    public MessageContent UnbanLog { get; set; } = "Player {username} ({userid}) has been unbanned by {playerid}";

    [Description("Sender here is just the nickname, it is not a player replacer because admin chat messages can be sent from non-players. If you want specific things to show up here you can use player as the player placeholder.")]
    public MessageContent AdminChatLog { get; set; } = "{sender} has sent an Admin Chat message of:\n`{message}`";
}