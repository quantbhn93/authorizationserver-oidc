using OpenIddict.Abstractions;
using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using AuthorizationServer.Customization.InMemorySQL.Models;
using AuthorizationServer.Customization.InMemorySQL.Stores;

namespace AuthorizationServer.Customization.InMemorySQL.Resolvers
{
    public class EpiOpenIdAuthorizationStoreResolver : IOpenIddictAuthorizationStoreResolver
    {
        private readonly TypeResolutionCache _cache;
        private readonly IServiceProvider _provider;

        public EpiOpenIdAuthorizationStoreResolver(TypeResolutionCache cache, IServiceProvider provider)
        {
            _cache = cache;
            _provider = provider;
        }

        public IOpenIddictAuthorizationStore<TAuthorization> Get<TAuthorization>() where TAuthorization : class
        {
            var store = _provider.GetService<IOpenIddictAuthorizationStore<TAuthorization>>();
            if (store != null)
            {
                return store;
            }

            var type = _cache.GetOrAdd(typeof(TAuthorization), key =>
            {
                if (!typeof(EpiOpenIdAuthorization).IsAssignableFrom(key))
                {
                    throw new InvalidOperationException($"{key} is invalid type for authorization.");
                }

                return typeof(EpiOpenIdAuthorizationStore<>).MakeGenericType(key);
            });

            return (IOpenIddictAuthorizationStore<TAuthorization>)_provider.GetRequiredService(type);
        }

        public class TypeResolutionCache : ConcurrentDictionary<Type, Type> { }
    }
}
