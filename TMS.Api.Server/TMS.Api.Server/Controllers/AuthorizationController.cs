using Microsoft.AspNetCore.Authentication;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using static OpenIddict.Abstractions.OpenIddictConstants;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Negotiate;
using System;
using TMS.Api.Server.Contracts;
using TMS.Api.Server.CustomExecption;
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace TMS.TokenGenerator.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthorizationController : ControllerBase
{
    private readonly IOpenIddictApplicationManager _applicationManager;
    private readonly IHandleRequest _handleRequest;

    public AuthorizationController(
        IOpenIddictApplicationManager applicationManager,
        IHandleRequest handleRequest)
    {

        this._applicationManager = applicationManager;
        this._handleRequest = handleRequest;
    }

    [HttpGet("~/connect/authorize")]
    [HttpPost("~/connect/authorize")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Authorize()
    {

        // Retrieve the Windows identity associated with the current authorization request.
        // If it can't be extracted, trigger an Integrated Windows Authentication dance.
        var result = await HttpContext.AuthenticateAsync(NegotiateDefaults.AuthenticationScheme);

        if (result is not { Succeeded: true })
        {
            return Challenge(
                authenticationSchemes: NegotiateDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties
                {
                    RedirectUri = Request.PathBase + Request.Path + QueryString.Create(
                        Request.HasFormContentType ? [.. Request.Form] : [.. Request.Query])
                });
        }

        var request = HttpContext.GetOpenIddictServerRequest();

        try
        {
            var claims = await _handleRequest.ChallengeRequestAsync(request, result);

            var identity = new ClaimsIdentity(claims,
               authenticationType: TokenValidationParameters.DefaultAuthenticationType,
               nameType: Claims.Name,
               roleType: Claims.Role
            );

            // The Windows identity doesn't contain the "sub" claim required by OpenIddict to represent
            // a stable identifier of the authenticated user. To work around that, a "sub" claim is
            // manually created by using the primary SID claim resolved from the Windows identity.
            var sid = identity.FindFirst(ClaimTypes.PrimarySid)?.Value;
            identity.AddClaim(new Claim(Claims.Subject, sid));
            identity.AddClaim(new Claim(Claims.Gender, "identified as men"));


            //identity.AddClaim(new Claim(Claims.c));

            // Allow all the claims resolved from the principal to be copied to the access and identity tokens.
            identity.SetDestinations(claim => [Destinations.AccessToken, Destinations.IdentityToken]);


            // Perform the sign-in operation.
            // HttpContext.SignInAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            // Retrieve the token (or manually set it as needed).
            var token = SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

            // Log the token (for debugging purposes).
            Console.WriteLine($"Generated Token: {token}");

            return SignIn(new ClaimsPrincipal(identity), OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

        }
        catch (ClientNotConfiguredException ex)
        {
            return Forbid(
                authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                properties: new AuthenticationProperties(new Dictionary<string, string?>
                {
                    [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.ServerError,
                    [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = ex.Message
                }));
        }
        catch(NoClientDetailsFoundExecption ex)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw;

        }

    }


}
