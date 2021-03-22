using Newtonsoft.Json.Linq;
using System.Collections.Immutable;
using System.Globalization;

namespace AuthorizationServer.Customization.InMemorySQL.Models
{
    public class EpiOpenIdScope
    {
        public string ScopeId { get; set; }

        public string Description { get; set; }

        public ImmutableDictionary<CultureInfo, string> Descriptions { get; set; } = ImmutableDictionary.Create<CultureInfo, string>();

        public string DisplayName { get; set; }

        public ImmutableDictionary<CultureInfo, string> DisplayNames { get; set; } = ImmutableDictionary.Create<CultureInfo, string>();

        /// <summary>
        /// Gets or sets the physical identifier associated with the current scope.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the unique name associated with the current scope.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     TODO; should we store as normal string and use System.Text.Json.JsonDocument.Parse(scope.Properties) to parse for properties in Store ?
        /// </summary>

        public JObject Properties { get; set; }

        public ImmutableArray<string> Resources { get; set; } = ImmutableArray.Create<string>();
    }
}
