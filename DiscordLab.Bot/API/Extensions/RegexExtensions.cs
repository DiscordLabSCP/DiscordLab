namespace DiscordLab.Bot.API.Extensions;

using System.Text.RegularExpressions;

/// <summary>
/// Utility extension methods for Regex.
/// </summary>
public static class RegexExtensions
{
    /// <summary>
    /// Checks if a string has the specific regex before calling the replacement method.
    /// </summary>
    /// <param name="regex">The regex to check.</param>
    /// <param name="input">The input string.</param>
    /// <param name="replacer">The replacement method.</param>
    /// <returns>The output string, replaced values depending on if regex matches.</returns>
    public static string CheckReplace(this Regex regex, string input, Func<string> replacer) =>
        regex.IsMatch(input) ? regex.Replace(input, replacer()) : input;
}