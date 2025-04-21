using Newtonsoft.Json;

namespace TMS.Api.Application.Model
{
    public class ApiResponse
    {
        [JsonProperty("message")]
        public string message { get; set; }

        [JsonProperty("response")]
        public ResponseData response { get; set; }
    }
}
