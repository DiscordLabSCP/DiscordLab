using Discord;
using Discord.WebSocket;

namespace DiscordLab.Bot.API.Extensions
{
    public static class ChannelExtensions
    {
        public static void SendMessageSync(this SocketTextChannel channel,
            string text = null, 
            // ReSharper disable once InconsistentNaming
            bool isTTS = false, 
            Embed embed = null, 
            RequestOptions options = null, 
            AllowedMentions allowedMentions = null, 
            MessageReference messageReference = null, 
            MessageComponent components = null, 
            ISticker[] stickers = null, 
            Embed[] embeds = null, 
            MessageFlags flags = MessageFlags.None, 
            PollProperties poll = null
        )
        {
            Task.Run(async () => 
                await channel.SendMessageAsync(
                    text,
                    isTTS,
                    embed,
                    options,
                    allowedMentions, 
                    messageReference, 
                    components, 
                    stickers, 
                    embeds, 
                    flags, 
                    poll
                )
            );
        }
    }
}