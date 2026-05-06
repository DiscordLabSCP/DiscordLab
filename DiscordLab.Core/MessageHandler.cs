namespace DiscordLab.Core;

public abstract partial class MessageHandler
{
    public static HashSet<MessageHandler> Handlers = new();

    public abstract void SendMessage(string destination, TranslationBuilder builder);
}