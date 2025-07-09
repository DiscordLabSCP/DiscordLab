namespace DiscordLab.Bot.API.Features.Embed
{
    using YamlDotNet.Serialization;

    /// <summary>
    /// Allows you to make an embed. Should be used in translations only.
    /// </summary>
    public class EmbedBuilder
    {
        /// <summary>
        /// Gets or sets the embed title.
        /// </summary>
        public string Title
        {
            get => Builder.Title;
            set => Builder.Title = value;
        }

        /// <summary>
        /// Gets or sets the embed description.
        /// </summary>
        public string Description
        {
            get => Builder.Description;
            set => Builder.Description = value;
        }

        /// <summary>
        /// Gets or sets the embed fields.
        /// </summary>
        public IEnumerable<EmbedFieldBuilder> Fields
        {
            get => Builder.Fields.Select(x => new EmbedFieldBuilder { Name = x.Name, Value = x.Value.ToString(), IsInline = x.IsInline });
            set => Builder.Fields = value.Select(x => x.Builder).ToList();
        }

        /// <summary>
        /// Gets or sets the color of the embed. In string so #, 0x or the raw hex value will work.
        /// </summary>
        public string Color
        {
            get => Builder.Color?.ToString();
            set => Builder.Color = Discord.Color.Parse(value);
        }

        [YamlIgnore]
        private Discord.EmbedBuilder Builder { get; } = new();

        /// <summary>
        /// Changes a <see cref="EmbedBuilder"/> into a <see cref="Discord.EmbedBuilder"/> instance.
        /// </summary>
        /// <param name="builder">The <see cref="EmbedBuilder"/> instance.</param>
        /// <returns>The <see cref="Discord.EmbedBuilder"/> instance.</returns>
        public static implicit operator Discord.EmbedBuilder(EmbedBuilder builder) =>
            builder.Builder;
    }
}