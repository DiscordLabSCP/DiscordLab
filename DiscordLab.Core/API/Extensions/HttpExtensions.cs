using System.Net.Http;
using System.Text.Json;

namespace DiscordLab.Core.API.Extensions;

public static class HttpExtensions
{
    extension(HttpContent content)
    {
        public async Task<T?> ReadFromJson<T>()
        {
            string raw = await content.ReadAsStringAsync();
            
            return JsonSerializer.Deserialize<T>(raw);
        }
    }
}