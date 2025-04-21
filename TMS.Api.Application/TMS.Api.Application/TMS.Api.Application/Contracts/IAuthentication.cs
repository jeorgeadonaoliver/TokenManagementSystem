namespace TMS.Api.Application.Contracts
{
    public interface IAuthentication
    {
        Task<bool> AuthenticateResponse(string id, string token);
    }
}
