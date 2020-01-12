using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FileCabinetApp.Extensions.ValidatorExtensions
{
    /// <summary>
    /// Class for complex Json deserialization.
    /// Contains favourite character constraints.
    /// </summary>
    public class FavCharConstraints
    {
        /// <summary>
        /// Gets or sets case of character. 0 if case insensitive,
        /// -1 if only lowercase allowed, 1 if only uppercase allowed.
        /// </summary>
        /// <value>Case of character: 0 if case insensitive,
        /// -1 if only lowercase allowed, 1 if only uppercase allowed.</value>
        [JsonPropertyName("symbolCase")]
        public int SymbolCase { get; set; }
    }
}
