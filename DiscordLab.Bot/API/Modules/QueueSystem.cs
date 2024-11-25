using MEC;

namespace DiscordLab.Bot.API.Modules
{
    public static class QueueSystem
    {
        private static List<string> _openQueueIds = new();
        
        /// <summary>
        /// Will run code after 5 seconds, but will only run once per id.
        /// This is different from <see cref="Timing.CallDelayed(float, System.Action)"/> because it will only run once
        /// 5 seconds after the initial call.
        /// </summary>
        /// <param name="id">
        /// A unique identifier for this queue. Like DiscordLab.BotStatus.PlayerVerified
        /// </param>
        /// <param name="action">
        /// The bit of code you want to run after 5 seconds, this shouldn't have any hard coded variables unless they
        /// will never change within the 5 seconds.
        /// </param>
        public static void QueueRun(string id, Action action)
        {
            if (_openQueueIds.Contains(id)) return;
            _openQueueIds.Add(id);
            Timing.CallDelayed(5, action);
        }
    }
}