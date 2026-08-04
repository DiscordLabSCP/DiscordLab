using DiscordLab.Core.API.Embed;
using DiscordLab.Core.API.TranslationBuilders;
using NorthwoodLib.Pools;

namespace DiscordLab.Core.API.Extensions;

public static class EmbedBuilderExtensions
{
    extension(EmbedBuilder builder)
    {
        public EmbedBuilder CloneWithTranslation(TranslationBuilder translation)
        {
            List<string> contents = ListPool<string>.Shared.Rent();
            
            contents.Add(builder.Title.OrIfEmpty());
            contents.Add(builder.Description.OrIfEmpty());
            contents.Add(builder.Url.OrIfEmpty());
            contents.Add(builder.ImageUrl.OrIfEmpty());
            contents.Add(builder.ThumbnailUrl.OrIfEmpty());
            contents.Add(builder.Footer?.Text.OrIfEmpty()!);
            contents.Add(builder.Footer?.IconUrl.OrIfEmpty()!);
        
            foreach (EmbedFieldBuilder field in builder.Fields ?? [])
            {
                contents.Add(field.Name.OrIfEmpty());
                contents.Add(field.Value.OrIfEmpty());
            }

            string delimiter = $"|{Guid.NewGuid()}|";
            string combined = string.Join(delimiter, contents);
            ListPool<string>.Shared.Return(contents);
            string replaced = translation.Build(combined);
            if (replaced.Split(delimiter) is not
                [
                    { } embedTitle, { } embedDescription, { } embedUrl, { } imageUrl, { } thumbnailUrl,
                    { } footer, { } iconUrl, .. { } fields
                ])
                throw new("Failed to build out the message content");

            EmbedBuilder embed = new()
            {
                Title = embedTitle,
                Description = embedDescription,
                Url = embedUrl,
                ImageUrl = imageUrl,
                ThumbnailUrl = thumbnailUrl,
                Footer = new()
                {
                    Text = footer,
                    IconUrl = iconUrl
                },
                Fields = new List<EmbedFieldBuilder>()
            };

            IEnumerable<string>[] fieldsChunked = fields.ChunkBy(2).ToArray();

            for (int i = 0; i < fieldsChunked.Length; i++)
            {
                string[] fieldsArr = fieldsChunked[i].ToArray();

                if (fieldsArr is not [{ } fieldName, { } fieldValue])
                    continue;

                if (embed.Fields is not List<EmbedFieldBuilder> fieldList)
                {
                    fieldList = new();
                    embed.Fields = fieldList;
                }
                
                fieldList.Add(new() { Name = fieldName, Value = fieldValue, IsInline = builder.Fields!.ElementAt(i).IsInline });
            }

            return embed;
        }
    }
}