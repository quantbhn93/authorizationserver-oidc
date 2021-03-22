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
    public class EpiOpenIdApplicationStore<TApplication> : IEpiOpenIdApplicationStore<TApplication> where TApplication : EpiOpenIdApplication, new()
    {

        private static IDictionary<string, TApplication> _memoryStore = new Dictionary<string, TApplication>();

        public EpiOpenIdApplicationStore() { }

        public ValueTask<long> CountAsync(CancellationToken cancellationToken) => new ValueTask<long>(_memoryStore.Count);

        public ValueTask<long> CountAsync<TResult>(Func<IQueryable<TApplication>, IQueryable<TResult>> query, CancellationToken cancellationToken)
           => throw new NotSupportedException();

        public virtual ValueTask CreateAsync(TApplication application, CancellationToken cancellationToken)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            cancellationToken.ThrowIfCancellationRequested();

            application.Id = _memoryStore.Count + 1;
            application.ApplicationId = System.Guid.NewGuid().ToString("n");
            _memoryStore.TryAdd(application.ApplicationId, application);

            return new ValueTask();
        }

        public virtual ValueTask DeleteAsync(TApplication application, CancellationToken cancellationToken)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            cancellationToken.ThrowIfCancellationRequested();
            if (_memoryStore.ContainsKey(application.ApplicationId))
            {
                _memoryStore.Remove(application.ApplicationId);
            }

            return new ValueTask();
        }

        public virtual ValueTask<TApplication> FindByClientIdAsync(string identifier, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));
            }
            cancellationToken.ThrowIfCancellationRequested();

            TApplication application = null;
            foreach (var pair in _memoryStore)
            {
                if (pair.Value.ClientId.Equals(identifier, StringComparison.OrdinalIgnoreCase))
                {
                    application = pair.Value;
                    break;
                }
            }

            return new ValueTask<TApplication>(application);
        }

        public virtual ValueTask<TApplication> FindByIdAsync(string identifier, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(identifier))
            {
                throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));
            }

            TApplication application = null;
            if (_memoryStore.ContainsKey(identifier))
            {
                _memoryStore.TryGetValue(identifier, out application);
            }

            return new ValueTask<TApplication>(application);
        }

        public virtual async IAsyncEnumerable<TApplication> FindByPostLogoutRedirectUriAsync(string address, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new ArgumentException("The address cannot be null or empty.", nameof(address));
            }
            cancellationToken.ThrowIfCancellationRequested();

            foreach (var pair in _memoryStore)
            {
                if (pair.Value.PostLogoutRedirectUris.Contains(address))
                {
                    await Task.Delay(0);
                    yield return pair.Value;
                }
            }
        }

        public virtual async IAsyncEnumerable<TApplication> FindByRedirectUriAsync(string address, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new ArgumentException("The address cannot be null or empty.", nameof(address));
            }

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var pair in _memoryStore)
            {
                if (pair.Value.RedirectUris.Contains(address))
                {
                    await Task.Delay(0);
                    yield return pair.Value;
                }
            }
        }

        public virtual ValueTask<TResult> GetAsync<TState, TResult>(Func<IQueryable<TApplication>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public virtual ValueTask<string> GetClientIdAsync(TApplication application, CancellationToken cancellationToken)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            return new ValueTask<string>(application.ClientId);
        }

        public virtual ValueTask<string> GetClientSecretAsync(TApplication application, CancellationToken cancellationToken)
        {
            return new ValueTask<string>(application?.ClientSecret);
        }

        public virtual ValueTask<string> GetClientTypeAsync(TApplication application, CancellationToken cancellationToken)
        {
            return new ValueTask<string>(application?.Type);
        }

        public virtual ValueTask<string> GetConsentTypeAsync(TApplication application, CancellationToken cancellationToken)
        {
            return new ValueTask<string>(application?.ConsentType);
        }

        public virtual ValueTask<string> GetDisplayNameAsync(TApplication application, CancellationToken cancellationToken)
        {
            return new ValueTask<string>(application?.DisplayName);
        }

        public virtual ValueTask<ImmutableDictionary<CultureInfo, string>> GetDisplayNamesAsync(TApplication application, CancellationToken cancellationToken)
        {
            // TODO 
            return new ValueTask<ImmutableDictionary<CultureInfo, string>>(ImmutableDictionary.Create<CultureInfo, string>());
        }

        public virtual ValueTask<string> GetIdAsync(TApplication application, CancellationToken cancellationToken)
        {
            return new ValueTask<string>(application?.ApplicationId);
        }

        public virtual ValueTask<ImmutableArray<string>> GetPermissionsAsync(TApplication application, CancellationToken cancellationToken)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }
            return new ValueTask<ImmutableArray<string>>(application.Permissions);
        }

        public virtual ValueTask<ImmutableArray<string>> GetPostLogoutRedirectUrisAsync(TApplication application, CancellationToken cancellationToken)
        {
            return new ValueTask<ImmutableArray<string>>(application.PostLogoutRedirectUris);
        }

        public virtual ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(TApplication application, CancellationToken cancellationToken)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            if (application.Properties == null)
            {
                return new ValueTask<ImmutableDictionary<string, JsonElement>>(ImmutableDictionary.Create<string, JsonElement>());
            }

            return new ValueTask<ImmutableDictionary<string, JsonElement>>(
                JsonSerializer.Deserialize<ImmutableDictionary<string, JsonElement>>(application.Properties.ToString()));
        }

        public virtual ValueTask<ImmutableArray<string>> GetRedirectUrisAsync(TApplication application, CancellationToken cancellationToken)
        {
            return new ValueTask<ImmutableArray<string>>(application.RedirectUris);
        }

        public virtual ValueTask<ImmutableArray<string>> GetRequirementsAsync(TApplication application, CancellationToken cancellationToken)
        {
            return new ValueTask<ImmutableArray<string>>(application.Requirements);
        }

        public virtual ValueTask<TApplication> InstantiateAsync(CancellationToken cancellationToken)
        {
            return new ValueTask<TApplication>(new TApplication { ApplicationId = Guid.NewGuid().ToString("n") });
        }

        public virtual async IAsyncEnumerable<TApplication> ListAsync(int? count, int? offset, CancellationToken cancellationToken)
        {
            foreach (var pair in _memoryStore)
            {
                await Task.Delay(0);
                yield return pair.Value;
            }
        }

        public IAsyncEnumerable<TResult> ListAsync<TState, TResult>(Func<IQueryable<TApplication>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public ValueTask SetClientIdAsync(TApplication application, string identifier, CancellationToken cancellationToken)
        {
            application.ClientId = identifier;
            return default;
        }

        public ValueTask SetClientSecretAsync(TApplication application, string secret, CancellationToken cancellationToken)
        {
            application.ClientSecret = secret;
            return default;
        }

        public ValueTask SetClientTypeAsync(TApplication application, string type, CancellationToken cancellationToken)
        {
            application.Type = type;
            return default;
        }

        public ValueTask SetConsentTypeAsync(TApplication application, string type, CancellationToken cancellationToken)
        {
            application.ConsentType = type;
            return default;
        }

        public ValueTask SetDisplayNameAsync(TApplication application, string name, CancellationToken cancellationToken)
        {
            application.DisplayName = name;
            return default;
        }

        public ValueTask SetDisplayNamesAsync(TApplication application, ImmutableDictionary<CultureInfo, string> names, CancellationToken cancellationToken)
        {
            application.DisplayName = names.FirstOrDefault().Value;
            return default;
        }

        public ValueTask SetPermissionsAsync(TApplication application, ImmutableArray<string> permissions, CancellationToken cancellationToken)
        {
            application.Permissions = permissions;
            return default;
        }

        public ValueTask SetPostLogoutRedirectUrisAsync(TApplication application, ImmutableArray<string> addresses, CancellationToken cancellationToken)
        {
            application.PostLogoutRedirectUris = addresses;
            return default;
        }

        public ValueTask SetPropertiesAsync(TApplication application, ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }

            if (properties == null || properties.IsEmpty)
            {
                application.Properties = null;
                return default;
            }

            application.Properties = JObject.Parse(JsonSerializer.Serialize(properties, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false
            }));

            return default;
        }

        public ValueTask SetRedirectUrisAsync(TApplication application, ImmutableArray<string> addresses, CancellationToken cancellationToken)
        {
            application.RedirectUris = addresses;
            return default;
        }

        public ValueTask SetRequirementsAsync(TApplication application, ImmutableArray<string> requirements, CancellationToken cancellationToken)
        {
            application.Requirements = requirements;
            return default;
        }

        public ValueTask UpdateAsync(TApplication application, CancellationToken cancellationToken)
        {
            if (application == null)
            {
                throw new ArgumentNullException(nameof(application));
            }
            cancellationToken.ThrowIfCancellationRequested();

            _memoryStore[application.ApplicationId] = application;
            return default;
        }
    }
}
