using System.Text;
using LabApi.Loader;
using MEC;
using NorthwoodLib.Pools;
using Random = UnityEngine.Random;

namespace DiscordLab.Bot.Commands.Syncing;

public record struct LinkInstance(string UserId, ulong DiscordId, string ConnectionCode)
{
    private static List<LinkInstance> Instances { get; set; } = [];

    private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    
    private static string RandomString()
    {
        StringBuilder builder = StringBuilderPool.Shared.Rent();

        for (int i = 0; i < 8; i++)
        {
            builder.Append(Chars[Random.Range(0, Chars.Length)]);
        }

        string result = StringBuilderPool.Shared.ToStringReturn(builder);
        
        if(Instances.Any(instance => instance.ConnectionCode == result))
            result = RandomString();
        
        return result;
    }
    
    public static string CreateInstance(string userId, ulong discordId)
    {
        string random = RandomString();
        LinkInstance instance = new(userId, discordId, random);
        Instances.Add(instance);
        Timing.CallDelayed(300, () => Instances.Remove(instance));
        
        return random;
    }

    public static bool SaveIfExists(string connectionCode)
    {
        LinkInstance? instance = Instances.FirstOrDefault(instance => instance.ConnectionCode == connectionCode);
        // How can it not be null silly compiler.
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (!instance.HasValue)
            return false;
        
        Plugin.Instance.UserIdsConfig.Users.Add(instance.Value.UserId, instance.Value.DiscordId);
        Plugin.Instance.SaveConfig(Plugin.Instance.UserIdsConfig, "userids.yml");

        return true;
    }
}