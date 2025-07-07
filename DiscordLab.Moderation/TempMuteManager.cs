using DiscordLab.Bot.API.Attributes;
using DiscordLab.Bot.API.Features;
using LabApi.Events.Handlers;
using LabApi.Features.Wrappers;
using LabApi.Loader;
using MEC;
using VoiceChat;

namespace DiscordLab.Moderation
{
    public static class TempMuteManager
    {
        public static TempMuteConfig MuteConfig => Plugin.Instance.MuteConfig;
        
        public static Dictionary<string, CoroutineHandle> Handles { get; private set; } = new();
        
        [CallOnLoad]
        public static void Start()
        {
            Dictionary<string, DateTime> mutes = MuteConfig.Mutes;
            foreach (KeyValuePair<string, DateTime> dict in mutes)
            {
                TimeSpan time = dict.Value - DateTime.Now;
                if (time.TotalSeconds < 0)
                {
                    RemoveMute(dict.Key);
                    continue;
                }
                AddHandle(dict.Key, time);
            }
        }

        [CallOnUnload]
        public static void Stop()
        {
            foreach (KeyValuePair<string, CoroutineHandle> mutes in Handles)
            {
                Timing.KillCoroutines(mutes.Key);
            }

            Handles = null;
        }

        public static void AddHandle(string userId, DateTime time) =>
            AddHandle(userId, time - DateTime.Now);
        
        public static void AddHandle(string userId, TimeSpan time) =>
            Handles.Add(userId,
                Timing.CallDelayed((float)time.TotalSeconds, () => RemoveMute(userId)));

        public static void RemoveHandle(string userId)
        {
            if (!Handles.TryGetValue(userId, out CoroutineHandle handle))
                return;

            Timing.KillCoroutines(handle);
            Handles.Remove(userId);
        }

        public static DateTime GetExpireDate(string duration) =>
            GetExpireDate(Misc.RelativeTimeToSeconds(duration, 60));

        public static DateTime GetExpireDate(long duration) => DateTime.Now.AddSeconds(duration);
        
        public static void MutePlayer(Player player, DateTime time, Player sender = null) => 
            MutePlayer(player.UserId, time, sender?.ReferenceHub);

        public static void MutePlayer(string player, DateTime time, ReferenceHub sender = null)
        {
            sender ??= Server.Host?.ReferenceHub;
            VoiceChatMutes.IssueLocalMute(player);
            if(Player.TryGet(player, out Player p) && sender)
                PlayerEvents.OnMuted(new(p.ReferenceHub, sender, false));
            
            MuteConfig.Mutes.Add(player, time);
            AddHandle(player, time);
            Plugin.Instance.SaveConfig(MuteConfig, "mute-config.yml");
        }

        public static void RemoveMute(Player player, Player sender = null) =>
            RemoveMute(player.UserId, sender?.ReferenceHub);

        public static void RemoveMute(string player, ReferenceHub sender = null)
        {
            if (!VoiceChatMutes.Mutes.Contains(player))
                return;

            sender ??= Server.Host?.ReferenceHub;

            VoiceChatMutes.RevokeLocalMute(player);
            
            MuteConfig.Mutes.Remove(player);
            Plugin.Instance.SaveConfig(MuteConfig, "mute-config.yml");
            if(Player.TryGet(player, out Player p) && sender)
                PlayerEvents.OnUnmuted(new(p.ReferenceHub, sender, false));

            if (Handles.ContainsKey(player))
                RemoveHandle(player);
        }
    }
}