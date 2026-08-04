namespace DiscordLab.Core.API.Extensions;

public static class StringExtensions
{
    extension(string? str)
    {
        public string OrIfEmpty(string? replacer = null) => string.IsNullOrEmpty(str) ? replacer ?? string.Empty : str!;
    }
}