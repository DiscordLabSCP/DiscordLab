using System.Reflection.Emit;
using Discord;
using Discord.WebSocket;
using DiscordLab.AdminLogs.Handlers;
using Exiled.API.Features;
using HarmonyLib;
using NorthwoodLib.Pools;
using static HarmonyLib.AccessTools;

namespace DiscordLab.AdminLogs.Patches
{
    [HarmonyPatch(typeof(Log), nameof(Log.Error), typeof(object))]
    [HarmonyPatch(typeof(Log), nameof(Log.Error), typeof(string))]
    public class ErrorLogger
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = -2;
            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Call) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new (OpCodes.Dup),
                new (OpCodes.Call, Method(typeof(ErrorLogger), nameof(LogError))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        public static void LogError(string message)
        {
            try
            {
                SocketTextChannel channel = DiscordBot.Instance.GetErrorLogsChannel();
                if (channel == null) return;

                EmbedBuilder embed = new()
                {
                    Title = Plugin.Instance.Translation.Error,
                    Description = message
                };

                channel.SendMessageAsync(embed: embed.Build());
            }
            catch
            {
                // ignored
            }
        }
    }
}