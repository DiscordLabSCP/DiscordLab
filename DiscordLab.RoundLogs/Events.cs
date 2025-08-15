using Discord.WebSocket;
using DiscordLab.Bot;
using DiscordLab.Bot.API.Extensions;
using DiscordLab.Bot.API.Features;
using DiscordLab.Bot.API.Utilities;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.CustomHandlers;
using PlayerRoles;
using LabApi.Features.Console;
using LabApi.Features.Extensions;
using LabApi.Features.Wrappers;
using Respawning.Waves;

namespace DiscordLab.RoundLogs;

public class Events : CustomEventsHandler
{
    public static Config Config => Plugin.Instance.Config;

    public static Translation Translation => Plugin.Instance.Translation;
        
    public override void OnPlayerChangedRole(PlayerChangedRoleEventArgs ev)
    {
        if (ev.ChangeReason is RoleChangeReason.Respawn or RoleChangeReason.RoundStart
            or RoleChangeReason.RespawnMiniwave or RoleChangeReason.LateJoin or RoleChangeReason.Died or RoleChangeReason.Destroyed)
            return;
            
        Dictionary<string, Func<string>> customReplacers = new()
        {
            ["oldrole"] = () => ev.OldRole.GetFullName(),
            ["newrole"] = () => ev.NewRole.RoleName,
            ["reason"] = () => ev.ChangeReason.ToString(),
            ["spawnflags"] = () => string.Join(", ", ev.SpawnFlags.GetFlags())
        };

        SocketTextChannel channel;

        TranslationBuilder builder;
            
        if (ev.NewRole.Team == ev.OldRole.GetTeam() && ev.NewRole.Team == Team.SCPs)
        {
            if (Config.ScpSwapChannelId == 0)
                return;
            channel = Client.GetOrAddChannel(Config.ScpSwapChannelId);
            if (channel == null)
            {
                Logger.Error(LoggingUtils.GenerateMissingChannelMessage("SCP Swap logs", Config.ScpSwapChannelId, Config.GuildId));
                return;
            }

            builder = new(Translation.ScpSwapLog, "player", ev.Player)
            {
                CustomReplacers = customReplacers
            };
                
            channel.SendMessage(builder);
            return;
        }
            
        if (Config.RoleChangeChannelId == 0)
            return;

        if (!Client.TryGetOrAddChannel(Config.RoleChangeChannelId, out channel))
        {
            Logger.Error(LoggingUtils.GenerateMissingChannelMessage("Role change logs", Config.RoleChangeChannelId, Config.GuildId));
        }

        builder = new(Translation.RoleChangeLog, "player", ev.Player)
        {
            CustomReplacers = customReplacers
        };
            
        channel.SendMessage(builder);
    }

    public override void OnServerWaveRespawned(WaveRespawnedEventArgs ev)
    {
        bool isFoundation = ev.Wave is MtfWave or MiniMtfWave;

        if ((isFoundation && Config.NtfSpawnChannelId == 0) || (!isFoundation && Config.ChaosSpawnChannelId == 0))
            return;

        ulong channelId = isFoundation ? Config.NtfSpawnChannelId : Config.ChaosSpawnChannelId;

        if (!Client.TryGetOrAddChannel(channelId, out SocketTextChannel channel))
        {
            Logger.Error(LoggingUtils.GenerateMissingChannelMessage("Wave respawn logs", channelId, Config.GuildId));
            return;
        }

        TranslationBuilder builder = new(isFoundation ? Translation.NtfSpawn : Translation.ChaosSpawn)
        {
            PlayerListItem = Translation.PlayerListItem,
            PlayerList = ev.Players
        };
            
        channel.SendMessage(builder);
    }

    public override void OnPlayerCuffed(PlayerCuffedEventArgs ev)
    {
        if (Config.CuffedChannelId == 0)
            return;

        if (!Client.TryGetOrAddChannel(Config.CuffedChannelId, out SocketTextChannel channel))
        {
            Logger.Error(LoggingUtils.GenerateMissingChannelMessage("cuffed logs", Config.CuffedChannelId, Config.GuildId));
            return;
        }

        TranslationBuilder builder = new(Translation.Cuffed);

        builder.AddPlayer("target", ev.Target);
        builder.AddPlayer("player", ev.Player);
            
        channel.SendMessage(builder);
    }

    public override void OnPlayerUncuffed(PlayerUncuffedEventArgs ev)
    {
        if (Config.UncuffedChannelId == 0)
            return;

        if (!Client.TryGetOrAddChannel(Config.UncuffedChannelId, out SocketTextChannel channel))
        {
            Logger.Error(LoggingUtils.GenerateMissingChannelMessage("uncuff logs", Config.CuffedChannelId, Config.GuildId));
            return;
        }

        TranslationBuilder builder = new(Translation.Uncuffed);

        builder.AddPlayer("target", ev.Target);
        builder.AddPlayer("player", ev.Player);
            
        channel.SendMessage(builder);
    }

    public override void OnServerRoundStarted()
    {
        if (Config.RoundStartedChannelId == 0)
            return;

        if (!Client.TryGetOrAddChannel(Config.RoundStartedChannelId, out SocketTextChannel channel))
        {
            Logger.Error(LoggingUtils.GenerateMissingChannelMessage("round start logs", Config.CuffedChannelId, Config.GuildId));
            return;
        }

        TranslationBuilder builder = new(Translation.RoundStart);
            
        channel.SendMessage(builder);
    }

    public override void OnServerRoundEnded(RoundEndedEventArgs ev)
    {
        if (Config.RoundEndedChannelId == 0)
            return;

        if (!Client.TryGetOrAddChannel(Config.RoundEndedChannelId, out SocketTextChannel channel))
        {
            Logger.Error(LoggingUtils.GenerateMissingChannelMessage("round ended logs", Config.CuffedChannelId, Config.GuildId));
            return;
        }

        TranslationBuilder builder = new(Translation.RoundEnd);
            
        builder.CustomReplacers.Add("winner", () => ev.LeadingTeam.ToString());
            
        channel.SendMessage(builder);
    }

    public override void OnServerLczDecontaminationStarted()
    {
        if (Config.DecontaminationChannelId == 0)
            return;

        if (!Client.TryGetOrAddChannel(Config.DecontaminationChannelId, out SocketTextChannel channel))
        {
            Logger.Error(LoggingUtils.GenerateMissingChannelMessage("decontamination logs", Config.DecontaminationChannelId, Config.GuildId));
            return;
        }
            
        channel.SendMessage(new TranslationBuilder(Translation.Decontamination));
    }
}