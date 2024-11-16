using MEC;

namespace DiscordLab.Bot.API.Modules;

public static class QueueSystem
{
    private static List<string> _openQueueIds = new ();
    
    public static void QueueRun(string id, Action action)
    {
        if (_openQueueIds.Contains(id)) return;
        _openQueueIds.Add(id);
        Timing.CallDelayed(5, action);
    }
}