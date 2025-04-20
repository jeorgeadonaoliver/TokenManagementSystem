using Microsoft.AspNetCore.Authentication;
using OpenIddict.Abstractions;
using System.Security.Claims;

namespace TMS.Api.Server.Contracts
{
    public interface IHandleRequest
    {

        public Task<IEnumerable<Claim>> ChallengeRequestAsync(OpenIddictRequest openIddictrequest, AuthenticateResult result);

    }
}
