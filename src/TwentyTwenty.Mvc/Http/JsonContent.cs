using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace TwentyTwenty.Mvc.Http
{
    public class JsonContent : StringContent
    {
        public JsonContent(object obj) :
            base(JsonSerializer.Serialize(obj), Encoding.UTF8, "application/json")        { }
    }
}