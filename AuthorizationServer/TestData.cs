using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;

namespace AuthorizationServer
{
    public class TestData : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public TestData(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<DbContext>();
            await context.Database.EnsureCreatedAsync(cancellationToken);

            var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

            if (await manager.FindByClientIdAsync("postman", cancellationToken) is null)
            {
                await manager.CreateAsync(new OpenIddictApplicationDescriptor
                {
                    ClientId = "postman",
                    ClientSecret = "postman-secret", // ìf ClientType = Public => this can be omitted
                    DisplayName = "Postman",
                    Type = OpenIddictConstants.ClientTypes.Confidential, 
                    // since we are using Postman for testing purpose.
                    // The authorization code is sent here after successful authentication.
                    RedirectUris = { new Uri("https://oauth.pstmn.io/v1/callback") },
                    Permissions =
                    {
                        OpenIddictConstants.Permissions.Endpoints.Token,
                        OpenIddictConstants.Permissions.Endpoints.Authorization,

                        OpenIddictConstants.Permissions.GrantTypes.ClientCredentials, // allow client to use Client Credentials Follow
                        OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                        OpenIddictConstants.Permissions.GrantTypes.RefreshToken,

                        // The ultimate job of an OpenID Connect/OAuth token service is to control access to resources.
                        /*
                            The two fundamental resource types in IdentityServer are:
                              - identity resources: represent claims about a user like user ID, display name, email address etc…
                              - API resources: represent functionality a client wants to access. Typically, they are HTTP-based endpoints (aka APIs), but could be also message queuing endpoints or similar.
                         */
                        OpenIddictConstants.Permissions.Prefixes.Scope + "api", // allow clients to request api scope
                        OpenIddictConstants.Permissions.Prefixes.Scope + "api1",
                        OpenIddictConstants.Permissions.Prefixes.Scope + "api2",

                        OpenIddictConstants.Permissions.ResponseTypes.Code // response type using Authorization Code flow. See https://www.scottbrady91.com/OpenID-Connect/OpenID-Connect-Flows
                    },
                    Requirements =
                        {
                             OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange
                        }
                }, cancellationToken);
            }

            var scopeManager = scope.ServiceProvider.GetRequiredService<IOpenIddictScopeManager>();
            if (await scopeManager.FindByNameAsync("api1") == null)
            {
                var descriptor = new OpenIddictScopeDescriptor
                {
                    Name = "api1",
                    Resources =
                    {
                       "resource_server_1"
                    }
                };

                await scopeManager.CreateAsync(descriptor);
            }

            if (await scopeManager.FindByNameAsync("api2") == null)
            {
                var descriptor = new OpenIddictScopeDescriptor
                {
                    Name = "api2",
                    Resources =
                    {
                       "resource_server_2"
                    }
                };

                await scopeManager.CreateAsync(descriptor);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
