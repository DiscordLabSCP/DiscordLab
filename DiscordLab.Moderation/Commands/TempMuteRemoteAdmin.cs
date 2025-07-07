using CommandSystem;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Utilities;
using LabApi.Features.Wrappers;

namespace DiscordLab.Moderation.Commands
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class TempMuteRemoteAdmin : ICommand, IUsageProvider
    {
        public string Command { get; } = "tempmute";
        public string[] Aliases { get; } = ["tempm", "mutet"];
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

            if (!CommandUtils.TryGetPlayerFromUnparsed(arguments.At(0), out Player target))
            {
                response = Plugin.Instance.Translation.InvalidUser;
            }

            DateTime time = TempMuteManager.GetExpireDate(arguments.At(1));
            
            TempMuteManager.MutePlayer(target, time, player);

            TranslationBuilder builder = new(Plugin.Instance.Translation.TempMuteSuccess, "player", target)
            {
                Time = time
            };
            
            builder.CustomReplacers.Add("duration", () => arguments.At(1));

            response = builder;
            return true;
        }
    }
}