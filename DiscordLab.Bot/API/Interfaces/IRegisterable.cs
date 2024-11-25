namespace DiscordLab.Bot.API.Interfaces
{
    public interface IRegisterable
    {
        /// <summary>
        /// Here is where you put your code that you want to run at the same time as OnEnabled in your plugin root.
        /// Useful things would be stuff like binding your static Instance variable.
        /// </summary>
        public void Init();
        
        /// <summary>
        /// Here is what you want to run when the plugin is just above to disable.
        /// </summary>
        public void Unregister();
    }
}