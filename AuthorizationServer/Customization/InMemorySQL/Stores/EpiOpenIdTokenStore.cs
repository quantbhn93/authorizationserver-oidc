using AuthorizationServer.Customization.Abstractions.Stores;
using AuthorizationServer.Customization.InMemorySQL.Models;
using Newtonsoft.Json.Linq;
using OpenIddict.Abstractions;
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
    public class EpiOpenIdTokenStore<TToken> : IEpiOpenIdTokenStore<TToken> where TToken : EpiOpenIdToken, new()
    {
        private static IDictionary<string, TToken> _memoryStore = new Dictionary<string, TToken>();

        public EpiOpenIdTokenStore() { }

        public virtual ValueTask<long> CountAsync(CancellationToken cancellationToken) => new ValueTask<long>(_memoryStore.Count);

        public virtual ValueTask<long> CountAsync<TResult>(Func<IQueryable<TToken>, IQueryable<TResult>> query, CancellationToken cancellationToken) => throw new NotSupportedException();

        public virtual ValueTask CreateAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(token.TokenId))
            {
                token.TokenId = System.Guid.NewGuid().ToString("n");
            }

            if (token.Id <= 0)
            {
                token.Id = _memoryStore.Count + 1;
            }

            _memoryStore[token.TokenId] = token;
            return default;
        }

        public virtual ValueTask DeleteAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            cancellationToken.ThrowIfCancellationRequested();
            if (_memoryStore.ContainsKey(token.TokenId))
            {
                _memoryStore.Remove(token.TokenId);
            }

            return default;
        }

        public virtual async IAsyncEnumerable<TToken> FindAsync(string subject, string client, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentException("The subject cannot be null or empty.", nameof(subject));
            }

            if (string.IsNullOrWhiteSpace(client))
            {
                throw new ArgumentException("The client cannot be null or empty.", nameof(client));
            }

            cancellationToken.ThrowIfCancellationRequested();
            foreach (var pair in _memoryStore)
            {
                if (pair.Value.ApplicationId.Equals(client, StringComparison.OrdinalIgnoreCase)
                    && pair.Value.Subject.Equals(subject, StringComparison.OrdinalIgnoreCase))
                {
                    await Task.Delay(0);
                    yield return pair.Value;
                }
            }
        }

        public virtual async IAsyncEnumerable<TToken> FindAsync(string subject, string client, string status, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentException("The subject cannot be null or empty.", nameof(subject));
            }

            if (string.IsNullOrWhiteSpace(client))
            {
                throw new ArgumentException("The client cannot be null or empty.", nameof(client));
            }

            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ArgumentException("The status cannot be null or empty.", nameof(status));
            }

            cancellationToken.ThrowIfCancellationRequested();
            foreach (var pair in _memoryStore)
            {
                if (pair.Value.ApplicationId.Equals(client, StringComparison.OrdinalIgnoreCase)
                    && pair.Value.Subject.Equals(subject, StringComparison.OrdinalIgnoreCase)
                    && pair.Value.Status.Equals(status, StringComparison.OrdinalIgnoreCase))
                {
                    await Task.Delay(0);
                    yield return pair.Value;
                }
            }
        }

        public virtual async IAsyncEnumerable<TToken> FindAsync(string subject, string client, string status, string type, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentException("The subject cannot be null or empty.", nameof(subject));
            }

            if (string.IsNullOrWhiteSpace(client))
            {
                throw new ArgumentException("The client identifier cannot be null or empty.", nameof(client));
            }

            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ArgumentException("The status cannot be null or empty.", nameof(status));
            }

            if (string.IsNullOrWhiteSpace(type))
            {
                throw new ArgumentException("The type cannot be null or empty.", nameof(type));
            }

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var pair in _memoryStore)
            {
                if (pair.Value.ApplicationId.Equals(client, StringComparison.OrdinalIgnoreCase)
                    && pair.Value.Subject.Equals(subject, StringComparison.OrdinalIgnoreCase)
                    && pair.Value.Status.Equals(status, StringComparison.OrdinalIgnoreCase)
                    && pair.Value.Type.Equals(type, StringComparison.OrdinalIgnoreCase))
                {
                    await Task.Delay(0);
                    yield return pair.Value;
                }
            }
        }

        public virtual async IAsyncEnumerable<TToken> FindByApplicationIdAsync(string identifier, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(identifier))
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

        public virtual async IAsyncEnumerable<TToken> FindByAuthorizationIdAsync(string identifier, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));
            }

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var pair in _memoryStore)
            {
                if (pair.Value.AuthorizationId.Equals(identifier, StringComparison.OrdinalIgnoreCase))
                {
                    await Task.Delay(0);
                    yield return pair.Value;
                }
            }
        }

        public virtual ValueTask<TToken> FindByIdAsync(string identifier, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));
            }

            cancellationToken.ThrowIfCancellationRequested();
            if (_memoryStore.ContainsKey(identifier))
            {
                return new ValueTask<TToken>(_memoryStore[identifier]);
            }

            return default;
        }

        public virtual ValueTask<TToken> FindByReferenceIdAsync(string identifier, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(identifier))
            {
                throw new ArgumentException("The identifier cannot be null or empty.", nameof(identifier));
            }

            cancellationToken.ThrowIfCancellationRequested();

            foreach (var pair in _memoryStore)
            {
                if (pair.Value.ReferenceId != null && pair.Value.ReferenceId.Equals(identifier, StringComparison.OrdinalIgnoreCase))
                {
                    return new ValueTask<TToken>(pair.Value);
                }
            }

            return default;
        }

        public virtual async IAsyncEnumerable<TToken> FindBySubjectAsync(string subject, CancellationToken cancellationToken)
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

        public virtual ValueTask<string> GetApplicationIdAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            return new ValueTask<string>(token.ApplicationId?.ToString(CultureInfo.InvariantCulture));
        }

        public virtual ValueTask<TResult> GetAsync<TState, TResult>(Func<IQueryable<TToken>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public virtual ValueTask<string> GetAuthorizationIdAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            return new ValueTask<string>(token.AuthorizationId);
        }

        public virtual ValueTask<DateTimeOffset?> GetCreationDateAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (token.CreationDate is null)
            {
                return new ValueTask<DateTimeOffset?>(result: null);
            }

            return new ValueTask<DateTimeOffset?>(DateTime.SpecifyKind(token.CreationDate.Value, DateTimeKind.Utc));
        }

        public virtual ValueTask<DateTimeOffset?> GetExpirationDateAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (token.ExpirationDate is null)
            {
                return new ValueTask<DateTimeOffset?>(result: null);
            }

            return new ValueTask<DateTimeOffset?>(DateTime.SpecifyKind(token.ExpirationDate.Value, DateTimeKind.Utc));
        }

        public virtual ValueTask<string> GetIdAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            return new ValueTask<string>(token.TokenId);
        }

        public virtual ValueTask<string> GetPayloadAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            return new ValueTask<string>(token.Payload);
        }

        public virtual ValueTask<ImmutableDictionary<string, JsonElement>> GetPropertiesAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (token.Properties == null)
            {
                return new ValueTask<ImmutableDictionary<string, JsonElement>>(ImmutableDictionary.Create<string, JsonElement>());
            }

            return new ValueTask<ImmutableDictionary<string, JsonElement>>(
                JsonSerializer.Deserialize<ImmutableDictionary<string, JsonElement>>(token.Properties.ToString()));
        }

        public virtual ValueTask<DateTimeOffset?> GetRedemptionDateAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (token.RedemptionDate is null)
            {
                return new ValueTask<DateTimeOffset?>(result: null);
            }

            return new ValueTask<DateTimeOffset?>(DateTime.SpecifyKind(token.RedemptionDate.Value, DateTimeKind.Utc));
        }

        public virtual ValueTask<string> GetReferenceIdAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            return new ValueTask<string>(token.ReferenceId);
        }

        public virtual ValueTask<string> GetStatusAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            return new ValueTask<string>(token.Status);
        }

        public virtual ValueTask<string> GetSubjectAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            return new ValueTask<string>(token.Subject);
        }

        public virtual ValueTask<string> GetTypeAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            return new ValueTask<string>(token.Type);
        }

        public virtual ValueTask<TToken> InstantiateAsync(CancellationToken cancellationToken)
        {
            return new ValueTask<TToken>(new TToken { TokenId = Guid.NewGuid().ToString("n"), Id = _memoryStore.Count + 1 });
        }

        public virtual async IAsyncEnumerable<TToken> ListAsync(int? count, int? offset, CancellationToken cancellationToken)
        {
            foreach (var pair in _memoryStore)
            {
                await Task.Delay(0);
                yield return pair.Value;
            }
        }

        public virtual IAsyncEnumerable<TResult> ListAsync<TState, TResult>(Func<IQueryable<TToken>, TState, IQueryable<TResult>> query, TState state, CancellationToken cancellationToken)
        {
            throw new NotSupportedException();
        }

        public virtual ValueTask PruneAsync(DateTimeOffset threshold, CancellationToken cancellationToken)
        {
            var keysToRemoved = new List<string>();

            foreach (var pair in _memoryStore)
            {
                var token = pair.Value;
                if (token.CreationDate < threshold.UtcDateTime
                    && ((token.Status != OpenIddictConstants.Statuses.Inactive && token.Status != OpenIddictConstants.Statuses.Valid)
                            || (token.ExpirationDate < DateTime.UtcNow)))

                {
                    keysToRemoved.Add(token.TokenId);
                }
            }

            if (keysToRemoved.Any())
            {
                keysToRemoved.ForEach(tokenId => _memoryStore.Remove(tokenId));
            }

            return default;
        }

        public virtual ValueTask SetApplicationIdAsync(TToken token, string identifier, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (string.IsNullOrEmpty(identifier))
            {
                token.ApplicationId = null;
            }
            else
            {
                token.ApplicationId = identifier;
            }

            return default;
        }

        public virtual ValueTask SetAuthorizationIdAsync(TToken token, string identifier, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (string.IsNullOrEmpty(identifier))
            {
                token.AuthorizationId = null;
            }
            else
            {
                token.AuthorizationId = identifier;
            }

            return default;
        }

        public virtual ValueTask SetCreationDateAsync(TToken token, DateTimeOffset? date, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            token.CreationDate = date?.UtcDateTime;
            return default;
        }

        public virtual ValueTask SetExpirationDateAsync(TToken token, DateTimeOffset? date, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            token.ExpirationDate = date?.UtcDateTime;
            return default;
        }

        public virtual ValueTask SetPayloadAsync(TToken token, string payload, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            token.Payload = payload;
            return default;
        }

        public virtual ValueTask SetPropertiesAsync(TToken token, ImmutableDictionary<string, JsonElement> properties, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (properties == null || properties.IsEmpty)
            {
                token.Properties = null;

                return default;
            }

            token.Properties = JObject.Parse(JsonSerializer.Serialize(properties, new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = false
            }));

            return default;
        }

        public virtual ValueTask SetRedemptionDateAsync(TToken token, DateTimeOffset? date, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            token.RedemptionDate = date?.UtcDateTime;
            return default;
        }

        public virtual ValueTask SetReferenceIdAsync(TToken token, string identifier, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            token.ReferenceId = identifier;
            return default;
        }

        public virtual ValueTask SetStatusAsync(TToken token, string status, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (string.IsNullOrWhiteSpace(status))
            {
                throw new ArgumentException("The status cannot be null or empty.", nameof(status));
            }

            token.Status = status;
            return default;
        }

        public virtual ValueTask SetSubjectAsync(TToken token, string subject, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (string.IsNullOrWhiteSpace(subject))
            {
                throw new ArgumentException("The subject cannot be null or empty.", nameof(subject));
            }

            token.Subject = subject;
            return default;
        }

        public virtual ValueTask SetTypeAsync(TToken token, string type, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }

            if (string.IsNullOrWhiteSpace(type))
            {
                throw new ArgumentException("The token type cannot be null or empty.", nameof(type));
            }

            token.Type = type;
            return default;
        }

        public virtual ValueTask UpdateAsync(TToken token, CancellationToken cancellationToken)
        {
            if (token == null)
            {
                throw new ArgumentNullException(nameof(token));
            }
            cancellationToken.ThrowIfCancellationRequested();

            _memoryStore[token.TokenId] = token;
            return default;
        }
    }
}
