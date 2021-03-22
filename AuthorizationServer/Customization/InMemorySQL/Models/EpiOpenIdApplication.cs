using Newtonsoft.Json.Linq;
using System.Collections.Immutable;

namespace AuthorizationServer.Customization.InMemorySQL.Models
{
    public class EpiOpenIdApplication
    {
        /// <summary>
        /// Gets or sets the physical identifier associated with the current application.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier associated with the current application.
        /// </summary>
        public string ApplicationId { get; set; }

        /// <summary>
        ///     Unique among others
        /// </summary>
        public string ClientId { get; set; }

        public string ClientSecret { get; set; }

        /// <summary>
        /// Gets or sets the consent type associated with the current application.
        /// </summary>
        public string ConsentType { get; set; }

        /// <summary>
        /// Gets or sets the display name associated with the current application.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets the permissions associated with the application.
        /// </summary>
        public ImmutableArray<string> Permissions { get; set; } = ImmutableArray.Create<string>();

        /// <summary>
        /// Gets the logout callback URLs associated with the current application.
        /// </summary>
        public ImmutableArray<string> PostLogoutRedirectUris { get; set; } = ImmutableArray.Create<string>();

        /// <summary>
        /// Gets or sets the additional properties associated with the current application.
        /// </summary>
        public JObject Properties { get; set; }

        /// <summary>
        /// Gets or sets the callback URLs associated with the current application.
        /// </summary>
        public ImmutableArray<string> RedirectUris { get; set; } = ImmutableArray.Create<string>();

        /// <summary>
        /// Gets or sets the requirements associated with the current application.
        /// </summary>
        public ImmutableArray<string> Requirements { get; set; } = ImmutableArray.Create<string>();

        /// <summary>
        /// Gets or sets the application type associated with the current application.
        /// <see cref=" OpenIddict.Abstractions.OpenIddictConstants.ClientTypes"/>
        /// </summary>
        public string Type { get; set; }
    }
}
