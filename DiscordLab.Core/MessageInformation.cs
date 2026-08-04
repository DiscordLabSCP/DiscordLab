using DiscordLab.Core.API.Embed;
using DiscordLab.Core.API.Features;
using DiscordLab.Core.API.TranslationBuilders;

namespace DiscordLab.Core;

public record struct MessageInformation(string? Content, IEnumerable<EmbedBuilder>? Embeds, Attachment? Attachment = null)
{
    public MessageInformation(string? content, EmbedBuilder? embed = null) : this(content, embed == null ? null : [embed]) {}

    public static implicit operator MessageInformation((string? content, EmbedBuilder? embed) tuple) => new(tuple.content, tuple.embed);
    
    public static implicit operator MessageInformation(string content) => new(content);

    public static implicit operator MessageInformation(EmbedBuilder embed) => new(null, embed);

    public static implicit operator MessageInformation(TranslationBuilder builder) => new(builder.Build());
}