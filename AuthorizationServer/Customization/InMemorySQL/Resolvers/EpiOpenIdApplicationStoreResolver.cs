using AuthorizationServer.Customization.InMemorySQL.Models;
using AuthorizationServer.Customization.InMemorySQL.Stores;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;
using System;
using System.Collections.Concurrent;

namespace AuthorizationServer.Customization.InMemorySQL.Resolvers
{
    public class EpiOpenIdApplicationStoreResolver : IOpenIddictApplicationStoreResolver
    {
        private readonly TypeResolutionCache _cache;
        private readonly IServiceProvider _provider;

        public EpiOpenIdApplicationStoreResolver(TypeResolutionCache cache, IServiceProvider provider)
        {
            _cache = cache;
            _provider = provider;
        }

        /// <inheritdoc/>
        public IOpenIddictApplicationStore<TApplication> Get<TApplication>() where TApplication : class
        {
            var store = _provider.GetService<IOpenIddictApplicationStore<TApplication>>();
            if (store != null)
            {
                return store;
            }

            var type = _cache.GetOrAdd(typeof(TApplication), key =>
            {
                if (!typeof(EpiOpenIdApplication).IsAssignableFrom(key))
                {
                    throw new InvalidOperationException($"{key} is invalid type for application.");
                }
                return typeof(EpiOpenIdApplicationStore<>).MakeGenericType(key);
            });

            return (IOpenIddictApplicationStore<TApplication>)_provider.GetRequiredService(type);
        }

        public class TypeResolutionCache : ConcurrentDictionary<Type, Type> { }
    }
}
