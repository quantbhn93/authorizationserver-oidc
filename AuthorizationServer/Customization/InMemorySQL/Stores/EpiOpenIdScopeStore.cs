using AuthorizationServer.Customization.Abstractions.Stores;
using AuthorizationServer.Customization.InMemorySQL.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationServer.Customization.InMemorySQL.Stores
{
    public class EpiOpenIdScopeStore<TScope> : IEpiOpenIdScopeStore<TScope> where TScope : EpiOpenIdScope, new()
    {
        private static IDictionary<string, TScope> _memoryStore = new Dictionary<string, TScope>();

        public EpiOpenIdScopeStore() { }

        public virtual ValueTask<long> CountAsync(CancellationToken cancellationToken) => new ValueTask<long>(_memoryStore.Count);

        public virtual ValueTask<long> CountAsync<TResult>(Func<IQueryable<TScope>, IQueryable<TResult>> query, CancellationToken cancellationToken) => throw new NotSupportedException();

        public virtual ValueTask CreateAsync(TScope scope, CancellationToken cancellationToken)
        {
            if (scope == null)
            {
                throw new ArgumentException(nameof(scope));
            }

            if (string.IsNullOrWhiteSpace(scope.ScopeId))
            {
                scope.ScopeId = System.Guid.NewGuid().ToString("n");
            }

            if (scope.Id <= 0)
            {
                scope.Id = _memoryStore.Count + 1;
            }

            _memoryStore[scope.ScopeId] = scope;
            return default;
        }

        public virtual ValueTask DeleteAsync(TScope scope, CancellationToken cancellationToken)
        {
            if (scope == null)
            {
                throw new ArgumentException(nameof(scope));
            }

            if (_memoryStore.ContainsKey(scope.ScopeId))
            {
                _memoryStore.Remove(scope.ScopeId);
            }

            return default;
        }

        public virtual ValueTask<TScope> FindByIdAsync(string identifier, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));
            }

            if (_memoryStore.ContainsKey(identifier))
            {
                return new ValueTask<TScope>(_memoryStore[identifier]);
            }

            return default;
        }

        public virtual ValueTask<TScope> FindByNameAsync(string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("The scope name cannot be null or empty.", nameof(name));
            }

            cancellationToken.ThrowIfCancellationRequested();
            foreach (var pair in _memoryStore)
            {
                if (pair.Value.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return new ValueTask<TScope>(pair.Value);
                }
            }

            return default;
        }

        public virtual async IAsyncEnumerable<TScope> FindByNamesAsync(ImmutableArray<string> names, CancellationToken cancellationToken)
        {
            if (names == null || names.Any(name => string.IsNullOrEmpty(name)))
            {
                throw new ArgumentException("Scope names cannot be null or empty.", nameof(names));
            }

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var pair in _memoryStore)
            {
                if (names.Contains(pair.Value.Name))
                {
                    await Task.Delay(0);
                    yield return pair.Value;
                }
            }
        }

        public virtual async IAsyncEnumerable<TScope> FindByResourceAsync(string resource, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(resource))
            {
                throw new ArgumentException("The scope resource cannot be null or empty.", nameof(resource));
            }

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var pair in _memoryStore)
            {
                if (pair.Value.Resources != null && pair.Value.Resources.Contains(resource))
                {
                    await Task.Delay(0);
                    yield return pair.Value;
                }
            }
        }

        public virtual ValueTask<TResult> GetAsync<TState, TResult>(Func<IQueryable<TScope>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public virtual ValueTask<string> GetDescriptionAsync(TScope scope, CancellationToken cancellationToken)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            return new ValueTask<string>(scope.Description);
        }

        public virtual ValueTask<ImmutableDictionary<CultureInfo, string>> GetDescriptionsAsync(TScope scope, CancellationToken cancellationToken)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            if (scope.Descriptions == null)
            {
                return new ValueTask<ImmutableDictionary<CultureInfo, string>>(ImmutableDictionary.Create<CultureInfo, string>());
            }

            return new ValueTask<ImmutableDictionary<CultureInfo, string>>(scope.Descriptions);
        }

        public virtual ValueTask<string> GetDisplayNameAsync(TScope scope, CancellationToken cancellationToken)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            return new ValueTask<string>(scope.DisplayName);
        }

        public virtual ValueTask<ImmutableDictionary<CultureInfo, string>> GetDisplayNamesAsync(TScope scope, CancellationToken cancellationToken)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            if (scope.DisplayNames == null)
            {
                return new ValueTask<ImmutableDictionary<CultureInfo, string>>(ImmutableDictionary.Create<CultureInfo, string>());
            }

            return new ValueTask<ImmutableDictionary<CultureInfo, string>>(scope.DisplayNames);
        }

        public virtual ValueTask<string> GetIdAsync(TScope scope, CancellationToken cancellationToken)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            return new ValueTask<string>(scope.ScopeId);
        }

        public virtual ValueTask<string> GetNameAsync(TScope scope, CancellationToken cancellationToken)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            return new ValueTask<string>(scope.Name);
        }

        public virtual ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(TScope scope, CancellationToken cancellationToken)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            if (scope.Properties == null)
            {
                return new ValueTask<ImmutableDictionary<string, JsonElement>>(ImmutableDictionary.Create<string, JsonElement>());
            }

            return new ValueTask<ImmutableDictionary<string, JsonElement>>(
                JsonSerializer.Deserialize<ImmutableDictionary<string, JsonElement>>(scope.Properties.ToString()));
        }

        public virtual ValueTask<ImmutableArray<string>> GetResourcesAsync(TScope scope, CancellationToken cancellationToken)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            return new ValueTask<ImmutableArray<string>>(scope.Resources);
        }

        public virtual ValueTask<TScope> InstantiateAsync(CancellationToken cancellationToken)
        {
            return new ValueTask<TScope>(new TScope { ScopeId = Guid.NewGuid().ToString("n"), Id = _memoryStore.Count + 1 });
        }

        public virtual async IAsyncEnumerable<TScope> ListAsync(int? count, int? offset, CancellationToken cancellationToken)
        {
            foreach (var pair in _memoryStore)
            {
                await Task.Delay(0);
                yield return pair.Value;
            }
        }

        public virtual IAsyncEnumerable<TResult> ListAsync<TState, TResult>(Func<IQueryable<TScope>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public virtual ValueTask SetDescriptionAsync(TScope scope, string description, CancellationToken cancellationToken)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            scope.Description = description;

            return default;
        }

        public virtual ValueTask SetDescriptionsAsync(TScope scope, ImmutableDictionary<CultureInfo, string> descriptions, CancellationToken cancellationToken)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            scope.Descriptions = descriptions;

            return default;
        }

        public virtual ValueTask SetDisplayNameAsync(TScope scope, string name, CancellationToken cancellationToken)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            scope.DisplayName = name;

            return default;
        }

        public virtual ValueTask SetDisplayNamesAsync(TScope scope, ImmutableDictionary<CultureInfo, string> names, CancellationToken cancellationToken)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            scope.DisplayNames = names;

            return default;
        }

        public virtual ValueTask SetNameAsync(TScope scope, string name, CancellationToken cancellationToken)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            scope.Name = name;

            return default;
        }

        public virtual ValueTask SetPropertiesAsync(TScope scope, ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            if (properties == null || properties.IsEmpty)
            {
                scope.Properties = null;

                return default;
            }

            scope.Properties = JObject.Parse(JsonSerializer.Serialize(properties, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false
            }));

            return default;
        }

        public virtual ValueTask SetResourcesAsync(TScope scope, ImmutableArray<string> resources, CancellationToken cancellationToken)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            scope.Resources = resources;

            return default;
        }

        public virtual ValueTask UpdateAsync(TScope scope, CancellationToken cancellationToken)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }
            cancellationToken.ThrowIfCancellationRequested();

            _memoryStore[scope.ScopeId] = scope;
            return default;
        }
    }
}
