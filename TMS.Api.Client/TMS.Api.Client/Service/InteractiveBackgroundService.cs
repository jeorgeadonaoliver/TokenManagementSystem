﻿using OpenIddict.Client;
using System.Runtime.InteropServices;
using TMS.Api.Client.Model;
using static OpenIddict.Abstractions.OpenIddictConstants;
using static OpenIddict.Abstractions.OpenIddictExceptions;

namespace TMS.Api.Client.Service;

public class InteractiveBackgroundService : BackgroundService
{
    private readonly IHostApplicationLifetime _lifetime;
    private readonly OpenIddictClientService _service;


    public InteractiveBackgroundService(
        IHostApplicationLifetime lifetime,
        OpenIddictClientService service)
    {
        _lifetime = lifetime;
        _service = service;
    }

    public async Task<AuthenticateModel> StartAuthenticationAsync(CancellationToken cancellationToken) 
    { 
        var result = await ExecuteAsync(cancellationToken);
        //return result.ToString();
        return result;
    }

    protected override async Task<AuthenticateModel> ExecuteAsync(CancellationToken stoppingToken)
    {
        // Wait for the host to confirm that the application has started.
        //var source = new TaskCompletionSource<bool>();
        //using (_lifetime.ApplicationStarted.Register(static state => ((TaskCompletionSource<bool>)state!).SetResult(true), source))
        //{
        //    await source.Task;
        //}

        //System.Console.WriteLine("Press any key to start the authentication process.");
        //await Task.Run(System.Console.ReadKey).WaitAsync(stoppingToken);

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
            }) ;


            System.Console.WriteLine("Claims:");

            foreach (var claim in response.Principal.Claims)
            {
                System.Console.WriteLine("{0}: {1}", claim.Type, claim.Value);
            }

            AuthenticateModel _authenticateModel = new AuthenticateModel()
            {
                client_id = "TMS.Api.Client",
                token = response.BackchannelAccessToken ?? response.FrontchannelAccessToken
            };

            System.Console.WriteLine();
            System.Console.WriteLine("Access token:");
            System.Console.WriteLine();
            System.Console.WriteLine(response.BackchannelAccessToken ?? response.FrontchannelAccessToken);

            
            return _authenticateModel;
            //return response.BackchannelAccessToken ?? response.FrontchannelAccessToken ?? "No Access token available.";
        }

        catch (OperationCanceledException)
        {
            //System.Console.WriteLine("The authentication process was aborted.");
            //return "The authentication process was aborted.";
            throw;
        }

        catch (ProtocolException exception) when (exception.Error is Errors.AccessDenied)
        {
            //System.Console.WriteLine("The authorization was denied by the end user.");
            //return "The authorization was denied by the end user.";
            throw;
        }

        catch
        {
            //System.Console.WriteLine("An error occurred while trying to authenticate the user.");
            //return "An error occurred while trying to authenticate the user.";
            throw;
        }
    }
}
