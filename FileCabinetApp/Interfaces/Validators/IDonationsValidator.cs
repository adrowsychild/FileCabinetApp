using System;
using System.Collections.Generic;

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// Donations validator.
    /// </summary>
    public interface IDonationsValidator
    {
        /// <summary>
        /// Validates donations of user's input.
        /// </summary>
        /// <param name="donations">Donations to validate.</param>
        /// <returns>Whether donations are valid.</returns>
        public bool Validate(decimal donations);
    }
}
