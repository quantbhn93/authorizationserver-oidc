using Newtonsoft.Json.Linq;
using System;

namespace AuthorizationServer.Customization.InMemorySQL.Models
{
    public class EpiOpenIdToken
    {
        /// <summary>
        /// Gets or sets the unique identifier associated with the current token.
        /// </summary>
        public string TokenId { get; set; }

        public string ApplicationId { get; set; }

        public string AuthorizationId { get; set; }

        public DateTime? CreationDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        /// <summary>
        ///  The physical identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the payload of the current token, if applicable.
        /// Note: this property is only used for reference tokens
        /// and may be encrypted for security reasons.
        /// </summary>
        public string Payload { get; set; }

        /// <summary>
        /// Gets or sets the additional properties associated with the current token.
        /// </summary>
        public JObject Properties { get; set; }

        /// <summary>
        /// Gets or sets the redemption date of the current token.
        /// </summary>
        public DateTime? RedemptionDate { get; set; }

        /// <summary>
        /// Gets or sets the reference identifier associated
        /// with the current token, if applicable.
        /// Note: this property is only used for reference tokens
        /// and may be hashed or encrypted for security reasons.
        /// </summary>
        public string ReferenceId { get; set; }

        /// <summary>
        /// Gets or sets the status of the current token.
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// Gets or sets the subject associated with the current token.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the type of the current token.
        /// </summary>
        public string Type { get; set; }
    }
}
