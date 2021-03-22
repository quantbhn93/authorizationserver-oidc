using AuthorizationServer.Customization.Abstractions.Managers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Core;

namespace AuthorizationServer.Customization.Services.Managers
{
    public class EpiOpenIdTokenManager<TToken> : OpenIddictTokenManager<TToken>, IEpiOpenIdTokenManager where TToken : class
    {
        public EpiOpenIdTokenManager(
           IOpenIddictTokenCache<TToken> cache,
           ILogger<OpenIddictTokenManager<TToken>> logger,
           IOptionsMonitor<OpenIddictCoreOptions> options,
           IOpenIddictTokenStoreResolver resolver)
                : base(cache, logger, options, resolver) { }
    }
}
