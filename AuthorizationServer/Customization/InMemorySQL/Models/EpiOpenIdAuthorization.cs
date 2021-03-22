using Newtonsoft.Json.Linq;
using System;
using System.Collections.Immutable;

namespace AuthorizationServer.Customization.InMemorySQL.Models
{
    public class EpiOpenIdAuthorization
    {
        /// <summary>
        ///     Physical id of authorization
        /// </summary>
        public int Id { get; set; }

        public string AuthorizationId { get; set; }

        public string ApplicationId { get; set; }

        public DateTime? CreationDate { get; set; }

        public JObject Properties { get; set; }

        public ImmutableArray<string> Scopes { get; set; } = ImmutableArray.Create<string>();

        public string Status { get; set; }

        public string Subject { get; set; }

        /// <summary>
        ///      Gets or sets the type of the current authorization.
        /// </summary>
        public string Type { get; set; }
    }
}
