using System.Reflection.Emit;
using Discord;
using Discord.WebSocket;
using DiscordLab.ModerationLogs.Handlers;
using Exiled.API.Features;
using HarmonyLib;
using NorthwoodLib.Pools;
using RemoteAdmin;

using static HarmonyLib.AccessTools;

namespace DiscordLab.ModerationLogs.Patches
{
    [HarmonyPatch(typeof(CommandProcessor), nameof(CommandProcessor.ProcessQuery))]
    internal class RemoteAdminLogger
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            const int index = 0;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(RemoteAdminLogger), nameof(SendCommand))),
            });

            foreach (CodeInstruction t in newInstructions)
                yield return t;

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static void SendCommand(string query, CommandSender sender)
        {
            if (Plugin.Instance.Config.RemoteAdminChannelId == 0) return;

            if (query.StartsWith("$")) return;
            
            Player player = sender is PlayerCommandSender commandSender
                ? Player.Get(commandSender)
                : Server.Host;
            if (player == null || string.IsNullOrEmpty(player.UserId)) return;

            SocketTextChannel channel = DiscordBot.Instance.GetRemoteAdminChannel();
            if (channel == null)
            {
                Log.Error("Either the guild is null or the channel is null. So the remote admin message has failed to send.");
                return;
            }

            EmbedBuilder embed = new()
            {
                Title = Plugin.Instance.Translation.RemoteAdminCommand,
                Color = Plugin.GetColor(Plugin.Instance.Config.RemoteAdminColor),
                Fields = new()
                {
                    new()
                    {
                        Name = Plugin.Instance.Translation.Command,
                        Value = query,
                        IsInline = false
                    },
                    new()
                    {
                        Name = Plugin.Instance.Translation.Issuer,
                        Value = player.Nickname,
                    },
                    new()
                    {
                        Name = Plugin.Instance.Translation.IssuerId,
                        Value = player.UserId,
                    }
                }
            };

            channel.SendMessageAsync(embed: embed.Build());
        }
    }
}