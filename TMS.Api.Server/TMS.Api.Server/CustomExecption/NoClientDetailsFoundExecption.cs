namespace TMS.Api.Server.CustomExecption
{
    public class NoClientDetailsFoundExecption : Exception
    {
        public NoClientDetailsFoundExecption(string message) : base(message) { }
    }
}
