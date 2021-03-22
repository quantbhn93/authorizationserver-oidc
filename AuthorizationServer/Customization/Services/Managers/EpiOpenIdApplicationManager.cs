using AuthorizationServer.Customization.Abstractions.Managers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Core;

namespace AuthorizationServer.Customization.Services.Managers
{
    public class EpiOpenIdApplicationManager<TApplication> : OpenIddictApplicationManager<TApplication>, IEpiOpenIdApplicationManager where TApplication : class
    {
        public EpiOpenIdApplicationManager(
                IOpenIddictApplicationCache<TApplication> cache,
                ILogger<EpiOpenIdApplicationManager<TApplication>> logger,
                IOptionsMonitor<OpenIddictCoreOptions> options,
                IOpenIddictApplicationStoreResolver resolver)
        : base(cache, logger, options, resolver) { }
    }
}
