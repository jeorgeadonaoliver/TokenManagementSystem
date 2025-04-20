using System;

namespace TMS.Api.Server.CustomExecption;

public class ClientNotConfiguredException : Exception
{
    public ClientNotConfiguredException(string message) : base(message) { }
}
