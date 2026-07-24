using System.Globalization;
using System.Text.RegularExpressions;
using DiscordLab.Core.API.Extensions;
using LabApi.Features.Wrappers;
using Mirror.LiteNetLib4Mirror;
using PlayerRoles;

namespace DiscordLab.Core.API.TranslationBuilders;

public class PlayerTranslationBuilder : TranslationBuilder
{
    /// <summary>
    /// Gets player based replacements.
    /// </summary>
    public static Dictionary<string, Func<Player, string>> PlayerReplacers { get; } = new()
    {
        ["name"] = static player =>
            player.Nickname.Replace("@everyone", "@\u200beveryone").Replace("@here", "@\u200bhere").Trim(),
        ["nickname"] = static player =>
            player.Nickname.Replace("@everyone", "@\u200beveryone").Replace("@here", "@\u200bhere").Trim(),
        ["displayname"] = static player => player.DisplayName,
        ["id"] = static player => player.UserId,
        ["ip"] = static player => player.IpAddress,
        ["userid"] = static player => player.PlayerId.ToString(),
        ["role"] = static player => player.RoleBase.RoleName,
        ["roletype"] = static player => player.Role.ToString(),
        ["team"] = static player => player.Team.ToString(),
        ["faction"] = static player => player.Team.GetFaction().ToString(),
        ["health"] = static player => player.Health.ToString(CultureInfo.CurrentCulture),
        ["maxhealth"] = static player => player.MaxHealth.ToString(CultureInfo.CurrentCulture),
        ["group"] = static player => player.GroupName,
        ["badgecolor"] = static player => player.GroupColor.ToString(),
        ["hasdnt"] = static player => player.DoNotTrack.ToString(),
        ["hasra"] = static player => player.RemoteAdminAccess.ToString(),
        ["isnorthwood"] = static player => player.IsNorthwoodStaff.ToString(),
        ["room"] = static player => player.Room?.ToString() ?? "None",
        ["zone"] = static player => player.Zone.ToString(),
        ["position"] = static player => player.Position.ToString(),
        ["ping"] = static player => LiteNetLib4MirrorServer.GetPing(player.Connection.connectionId).ToString(),
        ["isglobalmod"] = static player => player.IsGlobalModerator.ToString(),
        ["permissiongroup"] = static player => player.PermissionsGroupName ?? "None",
    };
    
    public PlayerTranslationBuilder() {}
    
    public PlayerTranslationBuilder(string translation) : base(translation) {}

    public PlayerTranslationBuilder(string playerKey, Player player)
    {
        AddPlayer(playerKey, player);
    }

    public PlayerTranslationBuilder(string translation, string playerKey, Player player) : this(playerKey, player)
    {
        Translation = translation;
    }

    /// <summary>
    /// Gets or sets the players that need to be translated for, if any.
    /// </summary>
    public Dictionary<string, Player> Players { get; set; } = new();

    /// <summary>
    /// Adds multiple players to the <see cref="Players"/> list.
    /// </summary>
    /// <param name="players">The players to add.</param>
    /// <returns>The <see cref="TranslationBuilder"/> instance.</returns>
    public PlayerTranslationBuilder AddPlayers(Dictionary<string, Player> players)
    {
        foreach (KeyValuePair<string, Player> pair in players)
        {
            Players.Add(pair.Key, pair.Value);
        }

        return this;
    }

    /// <summary>
    /// Adds a player to the <see cref="Players"/> list.
    /// </summary>
    /// <param name="prefix">The prefix for the player.</param>
    /// <param name="player">The <see cref="Player"/> to add.</param>
    /// <returns>The <see cref="TranslationBuilder"/> instance.</returns>
    public PlayerTranslationBuilder AddPlayer(string prefix, Player player)
    {
        Players.Add(prefix, player);

        return this;
    }

    /// <summary>
    /// Builds this <see cref="PlayerTranslationBuilder"/> instance.
    /// </summary>
    /// <param name="translation">The translation to build from, isn't needed if <see cref="TranslationBuilder.Translation"/> is defined.</param>
    /// <returns>The translation built.</returns>
    public new string Build(string? translation = null)
    {
        string output = base.Build(translation);
        
        foreach (KeyValuePair<string, Player> player in Players)
        {
            if (player.Value is not { IsReady: true })
                continue;

            Regex baseRegex = CachedRegex.GetOrAdd(player.Key, () => CreateRegex(player.Key));

            output = baseRegex.Replace(output, player.Value.Nickname);

            output = ReplacePlayer(output, player);
        }

        return output;
    }

    protected string ReplacePlayer(string message, KeyValuePair<string, Player> pair)
    {
        string output = message;
        
        foreach (KeyValuePair<string, Func<Player, string>> replacer in PlayerReplacers)
        {
            Regex regex = CachedRegex.GetOrAdd(
                $"{pair.Key}{replacer.Key}",
                () => CreateRegex($"{pair.Key}{replacer.Key}"));

            output = regex.CheckReplace(output, Replacement);
            continue;

            string Replacement() => GetReplacer(() => replacer.Value(pair.Value));
        }

        return output;
    }
}