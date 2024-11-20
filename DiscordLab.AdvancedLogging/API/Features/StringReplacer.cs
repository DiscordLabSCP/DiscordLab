using System.Reflection;

namespace DiscordLab.AdvancedLogging.API.Features;

public static class StringReplacer
{
    public static string Replacer(this string str, object ev)
    {
        if (ev == null) return str;
        string result = str;
        PropertyInfo[] properties = ev.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (PropertyInfo info in properties)
        {
            var value = info.GetValue(ev)?.ToString();
            if (value == null) continue;
            result = result.Replace($"{{{info.Name}}}", value);
        }

        return result;
    }
}