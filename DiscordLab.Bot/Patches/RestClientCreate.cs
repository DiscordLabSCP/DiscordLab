namespace DiscordLab.Bot.Patches;

using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Reflection.Emit;
using Discord.Net.Rest;
using HarmonyLib;
using LabApi.Features.Console;

/// <summary>
/// Patches <see cref="DefaultRestClient"/>.
/// </summary>
[HarmonyPatch]
public static class RestClientCreate
{
    /// <summary>
    /// Gets the target method to patch.
    /// </summary>
    /// <returns>The method.</returns>
    public static MethodBase TargetMethod()
    {
        foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type type = assembly.GetTypes().FirstOrDefault(t => t.Name == "DefaultRestClient");
            if (type == null)
                continue;
            ConstructorInfo constructor = type.GetConstructors().FirstOrDefault();
            if (constructor != null)
                return constructor;
        }

        return null;
    }

    /// <summary>
    /// The patch.
    /// </summary>
    /// <param name="instructions">The instructions.</param>
    /// <returns>The patched code.</returns>
    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        Logger.Debug("Transpiler start", Plugin.Instance.Config.Debug);

        CodeMatcher matcher = new CodeMatcher(instructions)
            .MatchEndForward(
                new CodeMatch(OpCodes.Ldarg_0),
                new CodeMatch(OpCodes.Newobj, AccessTools.Constructor(typeof(HttpClientHandler))),
                new CodeMatch(OpCodes.Dup),
                new CodeMatch(OpCodes.Ldc_I4_3),
                new CodeMatch(OpCodes.Callvirt),
                new CodeMatch(OpCodes.Dup),
                new CodeMatch(OpCodes.Ldc_I4_0),
                new CodeMatch(OpCodes.Callvirt),
                new CodeMatch(OpCodes.Dup),
                new CodeMatch(OpCodes.Ldarg_2),
                new CodeMatch(OpCodes.Callvirt),
                new CodeMatch(OpCodes.Dup),
                new CodeMatch(OpCodes.Ldarg_3),
                new CodeMatch(OpCodes.Callvirt));

        matcher.Advance(-13);

        matcher.RemoveInstructions(14)
            .Insert(
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldc_I4_3), // DecompressionMethods.GZip | DecompressionMethods.Deflate
                new CodeInstruction(OpCodes.Ldc_I4_0), // UseCookies = false
                new CodeInstruction(OpCodes.Ldarg_2),  // useProxy parameter
                new CodeInstruction(OpCodes.Ldarg_3),  // webProxy parameter
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(RestClientCreate), nameof(CreateHttpClientHandler))));

        Logger.Debug("Transpiler end", Plugin.Instance.Config.Debug);

        return matcher.InstructionEnumeration();
    }

    private static HttpClientHandler CreateHttpClientHandler(DecompressionMethods decompressionMethods, bool useCookies, bool useProxy, IWebProxy webProxy)
    {
        Logger.Debug("Creating HttpClientHandler", Plugin.Instance.Config.Debug);

        HttpClientHandler handler = new()
        {
            AutomaticDecompression = decompressionMethods,
            UseCookies = useCookies,
        };

        if (!useProxy)
            return handler;

        Logger.Debug("Creating HttpClientHandler with proxy", Plugin.Instance.Config.Debug);

        handler.UseProxy = true;
        handler.Proxy = webProxy;

        return handler;
    }
}