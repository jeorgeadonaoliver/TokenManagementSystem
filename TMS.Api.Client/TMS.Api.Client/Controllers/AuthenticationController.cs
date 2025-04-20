using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TMS.Api.Client.Service;

namespace TMS.Api.Client.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly InteractiveBackgroundService _interactiveService;
        public AuthenticationController(InteractiveBackgroundService interactiveService)
        {
            _interactiveService = interactiveService;
        }

        [HttpPost("StartAuthentication")]
        public async Task<IActionResult> StartAuthentication(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _interactiveService.StartAuthenticationAsync(cancellationToken);

                if (result == null)
                {
                    return BadRequest("Authentication process failed.");
                }

                return Ok(new
                {
                    Message = "Authentication process initiated. Check logs for details.",
                    //Nonce = result.Nonce
                    Response = result
                });
            }
            catch (OperationCanceledException)
            {
                return BadRequest("The authentication process was aborted.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }
}
