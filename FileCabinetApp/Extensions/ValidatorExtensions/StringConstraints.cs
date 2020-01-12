using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FileCabinetApp.Extensions.ValidatorExtensions
{
    /// <summary>
    /// Class for complex Json deserialization.
    /// Contains string constraints.
    /// </summary>
    public class StringConstraints
    {
        /// <summary>
        /// Gets or sets minimum possible length of the name.
        /// </summary>
        /// <value>Minimum possible length of the name.</value>
        [JsonPropertyName("minLength")]
        public int MinLength { get; set; }

        /// <summary>
        /// Gets or sets maximum possible length of the name.
        /// </summary>
        /// <value>Maximum possible length of the name.</value>
        [JsonPropertyName("maxLength")]
        public int MaxLength { get; set; }
    }
}
