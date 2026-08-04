using LabApi.Features.Wrappers;
using NorthwoodLib.Pools;
using YamlDotNet.Serialization;

namespace DiscordLab.Core.API.Commands;

public class GameUserCommandOptionBuilder : CommandOptionBuilder
{
    public GameUserCommandOptionBuilder()
    {
        Type = CommandOptionType.String;
        Autocomplete = information =>
        {
            List<(string name, string value)> list = new();
            foreach (Player player in Player.ReadyList)
            {
                if (!player.Nickname.Contains(information.Value))
                    continue;

                string name = $"{player.Nickname} ({player.PlayerId})";
                string value = player.UserId;

                if (ListType == ListType.All)
                    list.Add((name, value));
                else if (ListType == ListType.Players && !player.IsNpc)
                    list.Add((name, value));
                else if (ListType == ListType.Dummies && player.IsNpc)
                    list.Add((name, value));
            }

            return list;
        };
    }

    [YamlIgnore] public ListType ListType { get; set; } = ListType.All;
}

public enum ListType
{
    All,
    Players,
    Dummies
}