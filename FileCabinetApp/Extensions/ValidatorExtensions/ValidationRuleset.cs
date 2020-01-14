using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace FileCabinetApp.Extensions.ValidatorExtensions
{
    /// <summary>
    /// Class for complex Json deserialization.
    /// Contains validation ruleset for the  records' properties..
    /// </summary>
    public class ValidationRuleset
    {
        /// <summary>
        /// Gets or sets first name validation ruleset.
        /// </summary>
        /// <value>First name validation ruleset.</value>
        [JsonPropertyName("firstName")]
        public StringConstraints FirstName { get; set; }

        /// <summary>
        /// Gets or sets last name validation ruleset.
        /// </summary>
        /// <value>Last name validation ruleset.</value>
        [JsonPropertyName("lastName")]
        public StringConstraints LastName { get; set; }

        /// <summary>
        /// Gets or sets date of birth validation ruleset.
        /// </summary>
        /// <value>Date of birth validation ruleset.</value>
        [JsonPropertyName("dateOfBirth")]
        public AgeConstraints DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets favourite number validation ruleset.
        /// </summary>
        /// <value>Favourite number validation ruleset.</value>
        [JsonPropertyName("favouriteNumber")]
        public FavNumConstraints FavNumber { get; set; }

        /// <summary>
        /// Gets or sets favourite character validation ruleset.
        /// </summary>
        /// <value>Favourite character validation ruleset.</value>
        [JsonPropertyName("favouriteCharacter")]
        public FavCharConstraints FavCharacter { get; set; }

        /// <summary>
        /// Gets or sets favourite game validation ruleset.
        /// </summary>
        /// <value>Favourite game validation ruleset.</value>
        [JsonPropertyName("favouriteGame")]
        public StringConstraints FavGame { get; set; }

        /// <summary>
        /// Gets or sets donations validation ruleset.
        /// </summary>
        /// <value>Donations validation ruleset.</value>
        [JsonPropertyName("donations")]
        public DonationsConstraints Donations { get; set; }
    }
}
