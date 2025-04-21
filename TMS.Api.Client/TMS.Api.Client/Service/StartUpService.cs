using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using OpenIddict.Client;
using TMS.Api.Client.Database;

namespace TMS.Api.Client.Service;

public class StartUpService
{
    public IConfiguration _configuration { get; }
    public StartUpService(IConfiguration configuration)
    {
        this._configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection serviceCollection)
    {
        //serviceCollection.AddHostedService<InteractiveBackgroundService>();
        serviceCollection.AddSingleton<InteractiveBackgroundService>();

        //serviceCollection.AddControllersWithViews();
        serviceCollection.AddControllers();

        serviceCollection.AddDbContext<DbContext>(options =>
        {
            // Configure the context to use sqlite.
            options.UseSqlite($"Filename={Path.Combine(Path.GetTempPath(), "openiddict-joliver-server.sqlite3")}");

            // Register the entity sets needed by OpenIddict.
            // Note: use the generic overload if you need
            // to replace the default OpenIddict entities.
            options.UseOpenIddict();
        });

        serviceCollection.AddOpenIddict()
            // Register the OpenIddict core components.
            .AddCore(options =>
            {
                // Configure OpenIddict to use the Entity Framework Core stores and models.
                // Note: call ReplaceDefaultEntities() to replace the default OpenIddict entities.
                options.UseEntityFrameworkCore()
                       .UseDbContext<DbContext>();
            })

            // Register the OpenIddict client components.
            .AddClient(options =>
             {
                 // Note: this sample uses the authorization code flow,
                 // but you can enable the other flows if necessary.
                 options.AllowAuthorizationCodeFlow()
                        .AllowRefreshTokenFlow();

                 // Register the signing and encryption credentials used to protect
                 // sensitive data like the state tokens produced by OpenIddict.
                 options.AddDevelopmentEncryptionCertificate()
                        .AddDevelopmentSigningCertificate();

                 // Add the operating system integration.
                 options.UseSystemIntegration();

                 // Register the System.Net.Http integration and use the identity of the current
                 // assembly as a more specific user agent, which can be useful when dealing with
                 // providers that use the user agent as a way to throttle requests (e.g Reddit).
                 options.UseSystemNetHttp()
                        .SetProductInformation(typeof(Program).Assembly);

                 // Add a client registration matching the client application definition in the server project.
                 options.AddRegistration(new OpenIddictClientRegistration
                 {
                     Issuer = new Uri("https://localhost:7101/", UriKind.Absolute),

                     ClientId = "console_app",
                     RedirectUri = new Uri("/", UriKind.Relative),
                     ClientSecret = "ebat_adan"
                 });
             });

        serviceCollection.AddHttpClient("TMS.Api.Application", 
            client => { 
                client.BaseAddress = new Uri("https://localhost:7264/"); 
            }).ConfigurePrimaryHttpMessageHandler(() => 
            {
                return new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
            });


        // Register the worker responsible for creating the database used to store tokens
        // and adding the registry entries required to register the custom URI scheme.
        //
        // Note: in a real world application, this step should be part of a setup script.
        serviceCollection.AddHostedService<HostedService>();
        // Register the background service responsible for handling the console interactions.
        serviceCollection.AddHostedService<InteractiveBackgroundService>();

        serviceCollection.AddControllers();
    }
    
    public void Configure(IApplicationBuilder app)
    {
        app.UseDeveloperExceptionPage();

        app.UseRouting();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });



        //app.UseAuthentication();
        //app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapDefaultControllerRoute();
        });

        app.UseWelcomePage();
    }

}
    

