using System.ComponentModel;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Features.Embed;

namespace DiscordLab.Moderation;

public class Translation
{
    // String properties for command and option names
    public string MuteCommandName { get; set; } = "mute";
    public string MuteCommandDescription { get; set; } = "Mute a player on the server";
    public string MuteUserOptionName { get; set; } = "user";
    public string MuteUserOptionDescription { get; set; } = "The user to mute";
    public string MuteDurationOptionName { get; set; } = "duration";
    public string MuteDurationOptionDescription { get; set; } = "The duration to mute the user for";

    public string UnmuteCommandName { get; set; } = "unmute";
    public string UnmuteCommandDescription { get; set; } = "Unmute a player on the server";
    public string UnmuteUserOptionName { get; set; } = "user";
    public string UnmuteUserOptionDescription { get; set; } = "The user to unmute";

    public string BanCommandName { get; set; } = "ban";
    public string BanCommandDescription { get; set; } = "Ban a player on the server";
    public string BanUserOptionName { get; set; } = "user";
    public string BanUserOptionDescription { get; set; } = "The user to ban";
    public string BanDurationOptionName { get; set; } = "duration";
    public string BanDurationOptionDescription { get; set; } = "The duration to ban the user for";
    public string BanReasonOptionName { get; set; } = "reason";
    public string BanReasonOptionDescription { get; set; } = "The reason to ban the user";

    public string UnbanCommandName { get; set; } = "unban";
    public string UnbanCommandDescription { get; set; } = "Unban a player on the server";
    public string UnbanUserOptionName { get; set; } = "user";
    public string UnbanUserOptionDescription { get; set; } = "The user to unban";

    public string InvalidUser { get; set; } = "Please provide a valid user to use this command on.";

    public string TempMuteSuccess { get; set; } =
        "Player {player} has been temporarily muted for {duration}. They will get unmuted at {timef}";

    public string UnmuteSuccess { get; set; } = "Player {player} has been successfully unmuted.";

    public string PermMuteSuccess { get; set; } = "Player {player} has been muted.";

    public string BanFailure { get; set; } =
        "Failed to ban {userid}. Please make sure the data is valid and try again...";

    public string BanSuccess { get; set; } =
        "Successfully banned {userid} for {reason}. They will get unbanned in {timer}";

    public string UnbanSuccess { get; set; } = "Player {userid} has been unbanned.";

    public MessageContent PermMuteLog { get; set; } = "Player {target} has been muted by {player}.";

    public MessageContent TempMuteLog { get; set; } =
        "Player {target} has been muted by {player} for {timef}, they will be unmuted in {timer}";

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
}