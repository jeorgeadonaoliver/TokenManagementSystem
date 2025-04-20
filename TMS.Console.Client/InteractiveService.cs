
using System;
using Microsoft.Extensions.Hosting;
using OpenIddict.Client;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static OpenIddict.Abstractions.OpenIddictExceptions;

namespace TMS.TokenGenerator.Console.Client;

public class InteractiveService : BackgroundService
{
    private readonly IHostApplicationLifetime _lifetime;
    private readonly OpenIddictClientService _service;

    public InteractiveService(
        IHostApplicationLifetime lifetime,
        OpenIddictClientService service)
    {
        _lifetime = lifetime;
        _service = service;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // Wait for the host to confirm that the application has started.
        var source = new TaskCompletionSource<bool>();
        using (_lifetime.ApplicationStarted.Register(static state => ((TaskCompletionSource<bool>)state!).SetResult(true), source))
        {
            await source.Task;
        }

        System.Console.WriteLine("Press any key to start the authentication process.");
        await Task.Run(System.Console.ReadKey).WaitAsync(stoppingToken);

        try
        {
            // Ask OpenIddict to initiate the authentication flow (typically, by starting the system browser).
            var result = await _service.ChallengeInteractivelyAsync(new()
            {
                CancellationToken = stoppingToken
            });

            System.Console.WriteLine("System browser launched.");

            // Wait for the user to complete the authorization process.
            var response = await _service.AuthenticateInteractivelyAsync(new()
            {
                Nonce = result.Nonce
            });

            System.Console.WriteLine("Claims:");

            foreach (var claim in response.Principal.Claims)
            {
                System.Console.WriteLine("{0}: {1}", claim.Type, claim.Value);
            }

            System.Console.WriteLine();
            System.Console.WriteLine("Access token:");
            System.Console.WriteLine();
            System.Console.WriteLine(response.BackchannelAccessToken ?? response.FrontchannelAccessToken);
        }

        catch (OperationCanceledException)
        {
            System.Console.WriteLine("The authentication process was aborted.");
        }

        catch (ProtocolException exception) when (exception.Error is Errors.AccessDenied)
        {
            System.Console.WriteLine("The authorization was denied by the end user.");
        }

        catch
        {
            System.Console.WriteLine("An error occurred while trying to authenticate the user.");
        }
    }
}
