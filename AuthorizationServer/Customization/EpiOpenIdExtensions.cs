using AuthorizationServer.Customization.Abstractions.Managers;
using AuthorizationServer.Customization.InMemorySQL.Models;
using AuthorizationServer.Customization.InMemorySQL.Resolvers;
using AuthorizationServer.Customization.InMemorySQL.Stores;
using AuthorizationServer.Customization.Services.Managers;
using Microsoft.Extensions.DependencyInjection.Extensions;
using OpenIddict.Abstractions;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class EpiOpenIdExtensions
    {
        public static OpenIddictCoreBuilder UseEpiManagers (this OpenIddictCoreBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.ReplaceApplicationManager(typeof(EpiOpenIdApplicationManager<>));
            builder.ReplaceScopeManager(typeof(EpiOpenIdScopeManager<>));
            builder.ReplaceAuthorizationManager(typeof(EpiOpenIdAuthorizationManager<>));
            builder.ReplaceTokenManager(typeof(EpiOpenIdTokenManager<>));

            builder.Services.TryAddScoped(provider => (IEpiOpenIdApplicationManager)
                     provider.GetRequiredService<IOpenIddictApplicationManager>());
            builder.Services.TryAddScoped(provider => (IEpiOpenIdScopeManager)
                     provider.GetRequiredService<IOpenIddictScopeManager>());
            builder.Services.TryAddScoped(provider => (IEpiOpenIdAuthorizationManager)
                     provider.GetRequiredService<IOpenIddictAuthorizationManager>());
            builder.Services.TryAddScoped(provider => (IEpiOpenIdTokenManager)
                     provider.GetRequiredService<IOpenIddictTokenManager>());

            return builder;
        }

        public static OpenIddictCoreBuilder UseEpiInmemoryStore(this OpenIddictCoreBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.Configure(options => options.DisableAdditionalFiltering = false);

            builder.SetDefaultApplicationEntity<EpiOpenIdApplication>();
            builder.SetDefaultScopeEntity<EpiOpenIdScope>();
            builder.SetDefaultAuthorizationEntity<EpiOpenIdAuthorization>();
            builder.SetDefaultTokenEntity<EpiOpenIdToken>();

            builder.ReplaceApplicationStoreResolver<EpiOpenIdApplicationStoreResolver>();
            builder.ReplaceScopeStoreResolver<EpiOpenIdScopeStoreResolver>();
            builder.ReplaceAuthorizationStoreResolver<EpiOpenIdAuthorizationStoreResolver>();
            builder.ReplaceTokenStoreResolver<EpiOpenIdTokenStoreResolver>();

            builder.Services.TryAddSingleton<EpiOpenIdApplicationStoreResolver.TypeResolutionCache>();
            builder.Services.TryAddSingleton<EpiOpenIdScopeStoreResolver.TypeResolutionCache>();
            builder.Services.TryAddSingleton<EpiOpenIdAuthorizationStoreResolver.TypeResolutionCache>();
            builder.Services.TryAddSingleton<EpiOpenIdTokenStoreResolver.TypeResolutionCache>();

            builder.Services.TryAddScoped(typeof(EpiOpenIdApplicationStore<>));
            builder.Services.TryAddScoped(typeof(EpiOpenIdScopeStore<>));
            builder.Services.TryAddScoped(typeof(EpiOpenIdAuthorizationStore<>));
            builder.Services.TryAddScoped(typeof(EpiOpenIdTokenStore<>));

            return builder;
        }
    }
}
