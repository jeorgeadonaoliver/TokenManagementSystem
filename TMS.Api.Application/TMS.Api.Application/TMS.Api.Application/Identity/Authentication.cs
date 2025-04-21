using System.IdentityModel.Tokens.Jwt;
using TMS.Api.Application.Contracts;

namespace TMS.Api.Application.Identity
{
    public class Authentication : IAuthentication
    {
        public Task<bool> AuthenticateResponse(string id, string token)
        {
            if (id == "TMS.Api.Client")
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);

                foreach (var claim in jwt.Claims)
                {
                    //Console.WriteLine($"{claim.Type}: {claim.Value}");

                    if (claim.Type == "gender" && claim.Value == "identified as men") 
                    {
                        Console.WriteLine("LIFT YOUR WEIGHTS!!");
                    }
                }

                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}
