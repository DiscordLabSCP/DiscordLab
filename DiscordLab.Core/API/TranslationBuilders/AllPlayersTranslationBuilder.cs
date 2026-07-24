using LabApi.Features.Enums;
using LabApi.Features.Wrappers;

namespace DiscordLab.Core.API.TranslationBuilders;

public class AllPlayersTranslationBuilder : PlayerListTranslationBuilder
{
    public static Dictionary<string, Func<IEnumerable<Player>>> PlayerLists = new()
    {
        // All players, including dummies/NPCs
        ["players"] = static () => Player.ReadyList,
        // Only real players
        ["players no npcs"] = static () => Player.GetAll(PlayerSearchFlags.AuthenticatedPlayers),
        // Only NPCs/Dummies.
        ["npcs"] = static () => Player.NpcList,
        // Everything except NPCs hidden on player list
        ["players hidden npcs"] = static () => Player.ReadyList.Where(player => !player.IsDummy || !player.ReferenceHub.serverRoles.HideFromPlayerList)
    };

    public AllPlayersTranslationBuilder(string entry)
    {
        foreach (KeyValuePair<string, Func<IEnumerable<Player>>> pair in PlayerLists)
        {
            AddPlayersList(pair.Value, entry, pair.Key);
        }
    }

    public AllPlayersTranslationBuilder(string translation, string entry)
        : this(entry)
    {
        Translation = translation;
    }
}