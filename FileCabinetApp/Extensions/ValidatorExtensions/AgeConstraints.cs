using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FileCabinetApp.Extensions.ValidatorExtensions
{
    /// <summary>
    /// Class for complex Json deserialization.
    /// Contains age constraints.
    /// </summary>
    public class AgeConstraints
    {
        /// <summary>
        /// Gets or sets minimum possible age.
        /// </summary>
        /// <value>Minimum possible age.</value>
        [JsonPropertyName("from")]
        public int From { get; set; }

        /// <summary>
        /// Gets or sets maximum possible age.
        /// </summary>
        /// <value>Maximum possible age.</value>
        [JsonPropertyName("to")]
        public int To { get; set; }
    }
}
