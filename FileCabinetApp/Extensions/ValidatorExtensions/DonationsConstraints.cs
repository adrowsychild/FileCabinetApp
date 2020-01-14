using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace FileCabinetApp.Extensions.ValidatorExtensions
{
    /// <summary>
    /// Class for complex Json deserialization.
    /// Contains donations constraints.
    /// </summary>
    public class DonationsConstraints
    {
        /// <summary>
        /// Gets or sets minimum possible value.
        /// </summary>
        /// <value>Minimum possible value.</value>
        [JsonPropertyName("minValue")]
        public decimal MinValue { get; set; }
    }
}
