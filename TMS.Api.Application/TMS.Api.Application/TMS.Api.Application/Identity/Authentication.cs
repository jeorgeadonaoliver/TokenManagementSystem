using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using TMS.Api.Application.Contracts;

namespace TMS.Api.Application.Identity
{
    public class Authentication : IAuthentication
    {
        public async Task<string> AuthenticateRequest(string secret_key)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret_key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "TMS.Api.Client",
                audience: "TMS.Api.Application",
                //claims: new List<Claim> { new Claim(ClaimTypes.Email, "jaoliver@smits.sanmiguel.com.ph") },
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

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
