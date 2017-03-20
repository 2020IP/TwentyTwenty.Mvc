using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace TwentyTwenty.Mvc.Http
{
    public class ApiClient : HttpClient
    {
        public ApiClient()
            : base()
        {
            DefaultRequestHeaders.Accept.Clear();
            DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));            
        }

        public Task<HttpResponseMessage> PostAsJson(string requestUri, object obj)
            => this.PostAsync(requestUri, new JsonContent(obj));
    }
}