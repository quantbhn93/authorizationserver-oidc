using AuthorizationServer.Customization.Abstractions.Managers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenIddict.Abstractions;
using OpenIddict.Core;

namespace AuthorizationServer.Customization.Services.Managers
{
    public class EpiOpenIdScopeManager<TScope> : OpenIddictScopeManager<TScope>, IEpiOpenIdScopeManager where TScope : class
    {
        public EpiOpenIdScopeManager(
            IOpenIddictScopeCache<TScope> cache,
            ILogger<OpenIddictScopeManager<TScope>> logger,
            IOptionsMonitor<OpenIddictCoreOptions> options,
            IOpenIddictScopeStoreResolver resolver) : base(cache, logger, options, resolver)
        {

        }
    }
}
