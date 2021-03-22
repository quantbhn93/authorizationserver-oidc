using AuthorizationServer.Customization.Abstractions.Managers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Core;

namespace AuthorizationServer.Customization.Services.Managers
{
    public class EpiOpenIdAuthorizationManager<TAuthorization> : OpenIddictAuthorizationManager<TAuthorization>,
                                                                 IEpiOpenIdAuthorizationManager where TAuthorization : class
    {
        public EpiOpenIdAuthorizationManager(
            IOpenIddictAuthorizationCache<TAuthorization> cache,
            ILogger<OpenIddictAuthorizationManager<TAuthorization>> logger,
            IOptionsMonitor<OpenIddictCoreOptions> options,
            IOpenIddictAuthorizationStoreResolver resolver)
        : base(cache, logger, options, resolver)
        {
        }
    }
}
