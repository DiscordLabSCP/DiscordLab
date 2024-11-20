using System;

namespace DiscordLab.AdvancedLogging.API.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class Event : Attribute
    {
        public string Name { get; }

        public Event(string name)
        {
            Name = name;
        }
    }
}