using System.Reflection;
using System.Text.RegularExpressions;
using Discord.WebSocket;
using Exiled.API.Features;

namespace DiscordLab.AdvancedLogging.API.Modules;

public static class GenerateEvent
{
    public static void Event(object ev, SocketTextChannel channel, string content, IEnumerable<string> nullables)
    {
        List<string> nulls = nullables.ToList();
        
        Regex regex = new (@"\{([^\}]+)\}");
        MatchCollection matches = regex.Matches(content);

        foreach (Match match in matches)
        {
            string placeholder = match.Value;
            string propertyPath = match.Groups[1].Value;

            string[] properties = propertyPath.Split('.');

            object currentObject = ev;
            foreach (string property in properties)
            {
                if (currentObject == null) break;
                PropertyInfo propertyInfo = currentObject.GetType().GetProperty(property);
                if (propertyInfo == null && nulls.Contains(propertyPath))
                {
                    return;
                }
                currentObject = propertyInfo?.GetValue(currentObject);
            }

            if (currentObject != null)
            {
                content = content.Replace(placeholder, currentObject.ToString());
            }
        }

        channel.SendMessageAsync(content);
    }
}