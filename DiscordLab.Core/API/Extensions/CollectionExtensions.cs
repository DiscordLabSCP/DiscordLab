namespace DiscordLab.Core.API.Extensions;

public static class CollectionExtensions
{
    extension<T>(IEnumerable<T> collection)
    {
        public IEnumerable<IEnumerable<T>> ChunkBy(int chunkSize) => collection
            .Select((x, i) => new { Index = i, Value = x }).GroupBy(x => x.Index / chunkSize)
            .Select(x => x.Select(v => v.Value).ToList());
    }
}