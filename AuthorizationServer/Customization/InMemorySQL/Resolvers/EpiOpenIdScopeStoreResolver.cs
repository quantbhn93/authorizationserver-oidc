using OpenIddict.Abstractions;
using System;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using AuthorizationServer.Customization.InMemorySQL.Models;
using AuthorizationServer.Customization.InMemorySQL.Stores;

namespace AuthorizationServer.Customization.InMemorySQL.Resolvers
{
    public class EpiOpenIdScopeStoreResolver : IOpenIddictScopeStoreResolver
    {
        private readonly TypeResolutionCache _cache;
        private readonly IServiceProvider _provider;

        public EpiOpenIdScopeStoreResolver(TypeResolutionCache cache, IServiceProvider provider)
        {
            _cache = cache;
            _provider = provider;
        }

        public IOpenIddictScopeStore<TScope> Get<TScope>() where TScope : class
        {
            var store = _provider.GetService<IOpenIddictScopeStore<TScope>>();
            if (store != null)
            {
                return store;
            }

            var type = _cache.GetOrAdd(typeof(TScope), key =>
            {
                if (!typeof(EpiOpenIdScope).IsAssignableFrom(key))
                {
                    throw new InvalidOperationException($"The scope type must be assignable from {typeof(EpiOpenIdScope).FullName} ");
                }

                return typeof(EpiOpenIdScopeStore<>).MakeGenericType(key); // got only one generic parameter
            });

            return (IOpenIddictScopeStore<TScope>)_provider.GetRequiredService(type);
        }

        public class TypeResolutionCache : ConcurrentDictionary<Type, Type> { }
    }

}
