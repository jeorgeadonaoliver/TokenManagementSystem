using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Threading.Tasks;
using TMS.Api.Server.Contracts;
using Microsoft.AspNetCore.Http;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static System.Net.Mime.MediaTypeNames;
using OpenIddict.Core;
using System.Security.Claims;
using TMS.Api.Server.CustomExecption;

namespace TMS.Api.Server.Implementation;

public class HandleRequest : IHandleRequest
{
    private readonly IOpenIddictApplicationManager _openIddictApplicationManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
   
    public HandleRequest(
        IOpenIddictApplicationManager openIddictApplicationManager, 
        IHttpContextAccessor httpContextAccessor)
    {
        this._openIddictApplicationManager = openIddictApplicationManager;
        this._httpContextAccessor = httpContextAccessor;
    }
    private async Task<bool> VerifyRequestAsync(OpenIddictRequest openIddictrequest)
    { 
        var data = await GetDetailsFromDBAsync(openIddictrequest);

        var hasConsent = await _openIddictApplicationManager.HasConsentTypeAsync(data, ConsentTypes.Implicit);

        if (!hasConsent)
        {
            throw new ClientNotConfiguredException("The specified client application is not correctly configured.");
        }

        return true;
    }


    private async Task<OpenIddictRequest> ValidateOpenIddictRequestAsync(OpenIddictRequest request)
    {
        
        if (request is null)
        {
            throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");
        }

        return await Task.FromResult(request);
    }

    public async Task<IEnumerable<Claim>> ChallengeRequestAsync(OpenIddictRequest openIddictrequest, AuthenticateResult result)
    {
        var _request = await ValidateOpenIddictRequestAsync(openIddictrequest);

        var isVerified = await VerifyRequestAsync(openIddictrequest);

        var httpContext = _httpContextAccessor.HttpContext;

        if (isVerified)
        {
            return result.Principal.Claims;
        }

        return Enumerable.Empty<Claim>();
    }

    private async ValueTask<object> GetDetailsFromDBAsync(OpenIddictRequest openIddictrequest)
    {
        try
        {
            var data = await _openIddictApplicationManager.FindByClientIdAsync(openIddictrequest.ClientId);
            return data;
        }
        catch(Exception ex) 
        {
            throw new NoClientDetailsFoundExecption("Details concerning the calling client application cannot be found.");
        }

       
    }

}