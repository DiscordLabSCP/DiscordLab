namespace DiscordLab.Bot.API.Extensions;

using LabApi.Features.Console;

/// <summary>
/// Extension methods to help with Tasks.
/// </summary>
public static class TaskExtensions
{
#pragma warning disable SA1642
    /// <summary>
    /// Methods to manage tasks better.
    /// </summary>
#pragma warning restore SA1642
#pragma warning disable SA1400
    extension(Task)
#pragma warning restore SA1400
    {
        /// <summary>
        /// Runs and adds a logger to a Task.
        /// </summary>
        /// <param name="task">The Task to run.</param>
        /// <param name="onException">The action to run when an exception is triggered. If set, this will cancel the log from being sent naturally, so make sure to add your own log.</param>
        /// <returns>The task that is running.</returns>
        public static Task RunAndLog(Func<Task> task, Action<Exception>? onException = null) => Task.Run(async () =>
        {
            try
            {
                await task();
            }
            catch (Exception ex)
            {
                if (onException == null)
                    Logger.Error(ex);
                else
                    onException(ex);
            }
        });
    }
}