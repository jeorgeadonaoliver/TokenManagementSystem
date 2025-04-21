using Newtonsoft.Json;

namespace TMS.Api.Application.Model
{
    public class ResponseData
    {
        [JsonProperty("client_id")]
        public string client_id { get; set; }

        [JsonProperty("token")]
        public string token { get; set; }
    }
}
