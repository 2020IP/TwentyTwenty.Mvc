using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace TwentyTwenty.Mvc.Http
{
    public static class HttpClientExtensions
    {
        public static async Task<TResponse> ReadJson<TResponse>(this HttpResponseMessage msg)
        {
            var responseString = await msg.Content.ReadAsStringAsync();

            return responseString == null ? default : JsonSerializer.Deserialize<TResponse>(responseString);
        }
    }
}