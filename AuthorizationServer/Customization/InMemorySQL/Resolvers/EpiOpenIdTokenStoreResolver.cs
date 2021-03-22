using AuthorizationServer.Customization.InMemorySQL.Models;
using AuthorizationServer.Customization.InMemorySQL.Stores;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace AuthorizationServer.Customization.InMemorySQL.Resolvers
{
    public class EpiOpenIdTokenStoreResolver : IOpenIddictTokenStoreResolver
    {
        private readonly TypeResolutionCache _cache;
        private readonly IServiceProvider _provider;

        public EpiOpenIdTokenStoreResolver(TypeResolutionCache cache, IServiceProvider provider)
        {
            _cache = cache;
            _provider = provider;
        }

        /// <inheritdoc/>
        public IOpenIddictTokenStore<TToken> Get<TToken>() where TToken : class
        {
            var store = _provider.GetService<IOpenIddictTokenStore<TToken>>();
            if (store != null)
            {
                return store;
            }

            var type = _cache.GetOrAdd(typeof(TToken), key =>
            {
                if (!typeof(EpiOpenIdToken).IsAssignableFrom(key))
                {
                    throw new InvalidOperationException($"The token type must be assignable from {typeof(EpiOpenIdToken).FullName} ");
                }

                return typeof(EpiOpenIdTokenStore<>).MakeGenericType(key);
            });

            return (IOpenIddictTokenStore<TToken>)_provider.GetRequiredService(type);
        }

        public class TypeResolutionCache : ConcurrentDictionary<Type, Type> { }
    }
}
