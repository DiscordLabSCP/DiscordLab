using System.Text;
using LabApi.Features.Wrappers;
using NorthwoodLib.Pools;

namespace DiscordLab.Core.API.TranslationBuilders;

public class PlayerListTranslationBuilder : PlayerTranslationBuilder
{
    protected PlayerListTranslationBuilder() {}
    
    protected PlayerListTranslationBuilder(string translation) : base(translation) {}

    public PlayerListTranslationBuilder(IEnumerable<Player> players, string entry)
    {
        AddPlayersList(players, entry);
    }

    public PlayerListTranslationBuilder(string translation, IEnumerable<Player> players, string entry)
        : this(players, entry)
    {
        Translation = translation;
    }

    public void AddPlayersList(IEnumerable<Player> players, string entry, string customReplaceName = "players") =>
        AddPlayersList(() => players, entry, customReplaceName);
        
    
    public void AddPlayersList(Func<IEnumerable<Player>> playersFunc, string entry, string customReplaceName = "players") =>
        CustomReplacers.Add(CreateRegex(customReplaceName), () =>
        {
            StringBuilder builder = StringBuilderPool.Shared.Rent();

            foreach (Player player in playersFunc())
            {
                builder.AppendLine(ReplacePlayer(entry, new("player", player)));
            }

            return StringBuilderPool.Shared.ToStringReturn(builder);
        });
}