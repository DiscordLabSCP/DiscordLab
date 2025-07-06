namespace DiscordLab.Bot.API.Features
{
    using MEC;

    /// <summary>
    /// A utility class that helps with dealing with Discord rate limits by introducing a queue.
    /// </summary>
    public class Queue
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Queue"/> class.
        /// </summary>
        /// <param name="duration">The duration you wish to wait before calling the method.</param>
        /// <param name="defaultAction">The default action to run. Can be null.</param>
        public Queue(float duration, Action defaultAction = null)
        {
            Duration = duration;
            DefaultAction = defaultAction;
        }

        /// <summary>
        /// Gets the duration of time to wait between each call.
        /// </summary>
        public float Duration { get; }

        /// <summary>
        /// Gets a value indicating whether the queue is ongoing.
        /// </summary>
        public bool IsBusy { get; private set; }

        /// <summary>
        /// Gets the action to be run during <see cref="Process"/> when an action isn't provided. Can be null.
        /// </summary>
        public Action DefaultAction { get; }

        /// <summary>
        /// Runs the queue process, either using the action parameter or default action.
        /// </summary>
        /// <param name="action">The action to run. Defaults to <see cref="DefaultAction"/>.</param>
        public void Process(Action action = null)
        {
            if (IsBusy)
                return;

            action ??= DefaultAction;
            if (action == null)
                throw new ArgumentException("DefaultAction and parameter action can not be null at the same time.");

            IsBusy = true;

            Timing.CallDelayed(Duration, () =>
            {
                IsBusy = false;
                action();
            });
        }
    }
}