using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace TwentyTwenty.Mvc.Http
{
    public static class HttpClientExtensions
    {
        public static async Task<TResponse> ReadJson<TResponse>(this HttpResponseMessage msg)
        {
            var responseString = await msg.Content.ReadAsStringAsync();

            return responseString == null ? default(TResponse) : JsonConvert.DeserializeObject<TResponse>(responseString);
        }
    }
}