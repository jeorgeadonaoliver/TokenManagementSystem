using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TMS.Api.Application.Contracts;
using TMS.Api.Application.Model;

namespace TMS.Api.Application.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthentication _authentication;

        public AuthenticateController(
            IHttpClientFactory httpClientFactory,
            IAuthentication authentication)
        {
            this._httpClient = httpClientFactory.CreateClient("TMS.Api.Application");
            this._authentication = authentication;
        }

        [HttpGet("GetDataFrom7262Async")]
        public async Task<string> GetDataFrom7262Async()
        {
            var response = await _httpClient.GetAsync("https://localhost:7262/api/Authentication/GetToken");

            var content = await response.Content.ReadAsStringAsync();
            var parsedResponse = JsonConvert.DeserializeObject<ApiResponse>(content);

            var clientId = parsedResponse.response.client_id.ToString();
            var token = parsedResponse.response.token.ToString();

            var isAuthentic = _authentication.AuthenticateResponse(clientId, token);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to call API: {response.StatusCode} - {content}");
            }

            return content;
        }
       

    }

}
