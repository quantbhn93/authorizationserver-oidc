using AuthorizationServer.Customization.Abstractions.Stores;
using AuthorizationServer.Customization.InMemorySQL.Models;
using Newtonsoft.Json.Linq;
using OpenIddict.Abstractions;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationServer.Customization.InMemorySQL.Stores
{
    public class EpiOpenIdAuthorizationStore<TAuthorization> : IEpiOpenIdAuthorizationStore<TAuthorization> where TAuthorization : EpiOpenIdAuthorization, new()
    {
        private static IDictionary<string, TAuthorization> _memoryStore = new Dictionary<string, TAuthorization>();

        public EpiOpenIdAuthorizationStore() { }

        public virtual ValueTask<long> CountAsync(CancellationToken cancellationToken) => new ValueTask<long>(_memoryStore.Count);

        public virtual ValueTask<long> CountAsync<TResult>(Func<IQueryable<TAuthorization>, IQueryable<TResult>> query, CancellationToken cancellationToken)
          => throw new NotSupportedException();

        public virtual ValueTask CreateAsync(TAuthorization authorization, CancellationToken cancellationToken)
        {
            if (authorization == null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            cancellationToken.ThrowIfCancellationRequested();

            if (authorization.Id <= 0)
            {
                authorization.Id = _memoryStore.Count + 1;
            }
            if (string.IsNullOrWhiteSpace(authorization.AuthorizationId))
            {
                authorization.AuthorizationId = System.Guid.NewGuid().ToString("n");
            }
            _memoryStore.TryAdd(authorization.AuthorizationId, authorization);

            return default;
        }

        public virtual ValueTask DeleteAsync(TAuthorization authorization, CancellationToken cancellationToken)
        {
            if (authorization == null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            cancellationToken.ThrowIfCancellationRequested();

            if (_memoryStore.ContainsKey(authorization.AuthorizationId))
            {
                _memoryStore.Remove(authorization.AuthorizationId);
            }

            return default;
        }

        public virtual async IAsyncEnumerable<TAuthorization> FindAsync(string subject, string client, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(subject))
            {
                throw new ArgumentException("The subject cannot be null or empty.", nameof(subject));
            }

            if (string.IsNullOrEmpty(client))
            {
                throw new ArgumentException("The client cannot be null or empty.", nameof(client));
            }

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var pair in _memoryStore)
            {
                if (pair.Value.Subject.Equals(subject, StringComparison.OrdinalIgnoreCase)
                    && pair.Value.ApplicationId.Equals(client, StringComparison.OrdinalIgnoreCase))
                {
                    await Task.Delay(0);
                    yield return pair.Value;
                }
            }
        }

        public virtual async IAsyncEnumerable<TAuthorization> FindAsync(string subject, string client, string status, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(subject))
            {
                throw new ArgumentException("The subject cannot be null or empty.", nameof(subject));
            }

            if (string.IsNullOrEmpty(client))
            {
                throw new ArgumentException("The client identifier cannot be null or empty.", nameof(client));
            }

            if (string.IsNullOrEmpty(status))
            {
                throw new ArgumentException("The status cannot be null or empty.", nameof(client));
            }

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var pair in _memoryStore)
            {
                if (pair.Value.Subject.Equals(subject, StringComparison.OrdinalIgnoreCase)
                    && pair.Value.ApplicationId.Equals(client, StringComparison.OrdinalIgnoreCase)
                    && pair.Value.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
                {
                    await Task.Delay(0);
                    yield return pair.Value;
                }
            }
        }


        public virtual async IAsyncEnumerable<TAuthorization> FindAsync(string subject, string client, string status, string type, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(subject))
            {
                throw new ArgumentException("The subject cannot be null or empty.", nameof(subject));
            }

            if (string.IsNullOrEmpty(client))
            {
                throw new ArgumentException("The client identifier cannot be null or empty.", nameof(client));
            }

            if (string.IsNullOrEmpty(status))
            {
                throw new ArgumentException("The status cannot be null or empty.", nameof(client));
            }

            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentException("The type cannot be null or empty.", nameof(client));
            }

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var pair in _memoryStore)
            {
                if (pair.Value.Subject.Equals(subject, StringComparison.OrdinalIgnoreCase)
                    && pair.Value.ApplicationId.Equals(client, StringComparison.OrdinalIgnoreCase)
                    && pair.Value.Status.Equals(status, StringComparison.OrdinalIgnoreCase)
                    && pair.Value.Type.Equals(type, StringComparison.OrdinalIgnoreCase))
                {
                    await Task.Delay(0);
                    yield return pair.Value;
                }
            }
        }

        public virtual async IAsyncEnumerable<TAuthorization> FindAsync(string subject, string client, string status, string type, ImmutableArray<string> scopes, CancellationToken cancellationToken)
        {
            await foreach (var authorization in FindAsync(subject, client, status, type, cancellationToken))
            {
                if (new HashSet<string>(await GetScopesAsync(authorization, cancellationToken), StringComparer.Ordinal).IsSupersetOf(scopes))
                {
                    yield return authorization;
                }
            }
        }

        public virtual async IAsyncEnumerable<TAuthorization> FindByApplicationIdAsync(string identifier, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));
            }

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var pair in _memoryStore)
            {
                if (pair.Value.ApplicationId.Equals(identifier, StringComparison.OrdinalIgnoreCase))
                {
                    await Task.Delay(0);
                    yield return pair.Value;
                }
            }
        }

        public virtual ValueTask<TAuthorization> FindByIdAsync(string identifier, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));
            }

            TAuthorization authorization = null;
            if (_memoryStore.ContainsKey(identifier))
            {
                _memoryStore.TryGetValue(identifier, out authorization);
            }

            return new ValueTask<TAuthorization>(authorization);
        }

        public virtual async IAsyncEnumerable<TAuthorization> FindBySubjectAsync(string subject, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentException("The subject cannot be null or empty.", nameof(subject));
            }

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var pair in _memoryStore)
            {
                if (pair.Value.Subject.Equals(subject, StringComparison.OrdinalIgnoreCase))
                {
                    await Task.Delay(0);
                    yield return pair.Value;
                }
            }
        }

        public virtual ValueTask<string> GetApplicationIdAsync(TAuthorization authorization, CancellationToken cancellationToken)
        {
            if (authorization == null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            return new ValueTask<string>(authorization.ApplicationId);
        }

        public virtual ValueTask<TResult> GetAsync<TState, TResult>(Func<IQueryable<TAuthorization>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public virtual ValueTask<DateTimeOffset?> GetCreationDateAsync(TAuthorization authorization, CancellationToken cancellationToken)
        {
            if (authorization == null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            if (authorization.CreationDate is null)
            {
                return new ValueTask<DateTimeOffset?>(result: null);
            }

            return new ValueTask<DateTimeOffset?>(DateTime.SpecifyKind(authorization.CreationDate.Value, DateTimeKind.Utc));
        }

        public virtual ValueTask<string> GetIdAsync(TAuthorization authorization, CancellationToken cancellationToken)
        {
            if (authorization == null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            return new ValueTask<string>(authorization.AuthorizationId);
        }

        public virtual ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(TAuthorization authorization, CancellationToken cancellationToken)
        {
            if (authorization == null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            if (authorization.Properties == null)
            {
                return new ValueTask<ImmutableDictionary<string, JsonElement>>(ImmutableDictionary.Create<string, JsonElement>());
            }

            return new ValueTask<ImmutableDictionary<string, JsonElement>>(
                JsonSerializer.Deserialize<ImmutableDictionary<string, JsonElement>>(authorization.Properties.ToString()));
        }

        public virtual ValueTask<ImmutableArray<string>> GetScopesAsync(TAuthorization authorization, CancellationToken cancellationToken)
        {
            if (authorization == null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            return new ValueTask<ImmutableArray<string>>(authorization.Scopes);
        }

        public virtual ValueTask<string> GetStatusAsync(TAuthorization authorization, CancellationToken cancellationToken)
        {
            if (authorization == null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            return new ValueTask<string>(authorization.Status);
        }

        public virtual ValueTask<string> GetSubjectAsync(TAuthorization authorization, CancellationToken cancellationToken)
        {
            if (authorization == null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            return new ValueTask<string>(authorization.Subject);
        }

        public virtual ValueTask<string> GetTypeAsync(TAuthorization authorization, CancellationToken cancellationToken)
        {
            if (authorization == null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            return new ValueTask<string>(authorization.Type);
        }

        public virtual ValueTask<TAuthorization> InstantiateAsync(CancellationToken cancellationToken)
        {
            var authorization = new TAuthorization { AuthorizationId = Guid.NewGuid().ToString("n"), Id = _memoryStore.Count + 1 };
            return new ValueTask<TAuthorization>(authorization);
        }

        public virtual async IAsyncEnumerable<TAuthorization> ListAsync(int? count, int? offset, CancellationToken cancellationToken)
        {
            foreach (var pair in _memoryStore)
            {
                await Task.Delay(0);
                yield return pair.Value;
            }
        }

        public virtual IAsyncEnumerable<TResult> ListAsync<TState, TResult>(Func<IQueryable<TAuthorization>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public virtual ValueTask PruneAsync(DateTimeOffset threshold, CancellationToken cancellationToken)
        {
            var keysToRemoved = new List<string>();

            foreach (var pair in _memoryStore)
            {
                var authorization = pair.Value;
                if (authorization.CreationDate < threshold.UtcDateTime 
                    && authorization.Status != OpenIddictConstants.Statuses.Valid
                    && authorization.Type == OpenIddictConstants.AuthorizationTypes.AdHoc)
                {
                    keysToRemoved.Add(authorization.AuthorizationId);
                }
            }

            if (keysToRemoved.Any())
            {
                keysToRemoved.ForEach(authorizationId => _memoryStore.Remove(authorizationId));
            }

            return default;
        }

        public virtual ValueTask SetApplicationIdAsync(TAuthorization authorization, string identifier, CancellationToken cancellationToken)
        {
            if (authorization == null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            if (string.IsNullOrEmpty(identifier))
            {
                authorization.ApplicationId = null;
            }
            else
            {
                authorization.ApplicationId = identifier;
            }

            return default;
        }

        public virtual ValueTask SetCreationDateAsync(TAuthorization authorization, DateTimeOffset? date, CancellationToken cancellationToken)
        {
            if (authorization == null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            authorization.CreationDate = date?.UtcDateTime;
            return default;
        }

        public virtual ValueTask SetPropertiesAsync(TAuthorization authorization, ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
        {
            if (authorization == null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            if (properties == null || properties.IsEmpty)
            {
                authorization.Properties = null;

                return default;
            }

            authorization.Properties = JObject.Parse(JsonSerializer.Serialize(properties, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false
            }));

            return default;
        }

        public virtual ValueTask SetScopesAsync(TAuthorization authorization, ImmutableArray<string> scopes, CancellationToken cancellationToken)
        {
            if (authorization == null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            authorization.Scopes = scopes;
            return default;
        }

        public virtual ValueTask SetStatusAsync(TAuthorization authorization, string status, CancellationToken cancellationToken)
        {
            if (authorization == null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            authorization.Status = status;
            return default;
        }

        public virtual ValueTask SetSubjectAsync(TAuthorization authorization, string subject, CancellationToken cancellationToken)
        {
            if (authorization == null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            authorization.Subject = subject;
            return default;
        }

        public virtual ValueTask SetTypeAsync(TAuthorization authorization, string type, CancellationToken cancellationToken)
        {
            if (authorization == null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }

            authorization.Type = type;
            return default;
        }

        public virtual ValueTask UpdateAsync(TAuthorization authorization, CancellationToken cancellationToken)
        {
            if (authorization == null)
            {
                throw new ArgumentNullException(nameof(authorization));
            }
            cancellationToken.ThrowIfCancellationRequested();

            _memoryStore[authorization.AuthorizationId] = authorization;
            return default;
        }
    }
}
