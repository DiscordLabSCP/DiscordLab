using System.Text;
using CommandSystem;
using DiscordLab.Core.API.TranslationBuilders;
using LabApi.Features.Wrappers;
using NorthwoodLib.Pools;
using Utils;

namespace DiscordLab.Moderation.Commands;

public class TempMuteRemoteAdmin : ICommand, IUsageProvider
{
    public string Command { get; } = "tempmute";
    public string[] Aliases { get; } = ["tempm", "mutet", "temporarymute", "mutetemp", "mutetemporary"];
    public string Description { get; } = "Temporarily mutes a user.";

    public string[] Usage { get; } =
    [
        "player",
        "duration"
    ];

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (!sender.CheckPermission([
                PlayerPermissions.BanningUpToDay,
                PlayerPermissions.LongTermBanning,
                PlayerPermissions.PlayersManagement
            ], out response))
            return false;

        if (arguments.Count < 2)
        {
            response = "To execute this command provide at least 2 arguments!\nUsage: " + this.DisplayCommandUsage();
            return false;
        }

        if (!Player.TryGet(sender, out Player player))
        {
            player = Server.Host;
        }

        IEnumerable<ReferenceHub> players = RAUtils.ProcessPlayerIdOrNamesList(arguments, 0, out string[] newArgs);

        if (!players.Any())
        {
            response = Plugin.Instance.Translation.InvalidUser;
        }

        DateTime time = TempMuteManager.GetExpireDate(newArgs[0]);

        StringBuilder str = StringBuilderPool.Shared.Rent();
        
        foreach (ReferenceHub referenceHub in players)
        {
            Player target = Player.Get(referenceHub);
            TempMuteManager.MutePlayer(target, time, player);

            TranslationBuilder builder =
                new PlayerTranslationBuilder(Plugin.Instance.Translation.TempMuteSuccess, "player", target)
                    {
                        Time = time
                    }
                    .AddCustomReplacer("duration", () => newArgs[0]);

            str.AppendLine(builder);
        }

        response = StringBuilderPool.Shared.ToStringReturn(str);
        return true;
    }
}