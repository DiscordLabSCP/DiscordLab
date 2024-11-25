namespace DiscordLab.Bot.API.Features
{
    public class UpdateStatus
    {
        /// <summary>
        /// The name of the module/plugin.
        /// </summary>
        public string ModuleName { get; set; }
        /// <summary>
        /// The version of the module/plugin.
        /// </summary>
        public Version Version { get; set; }
        /// <summary>
        /// The download url of this version of the module/plugin.
        /// </summary>
        public string Url { get; set; }
    }
}