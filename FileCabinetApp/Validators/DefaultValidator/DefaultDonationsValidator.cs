using System;
using System.Collections.Generic;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators.DefaultValidator
{
    /// <summary>
    /// Donations validator.
    /// </summary>
    public class DefaultDonationsValidator : IDonationsValidator
    {
        /// <summary>
        /// Validates donations of user's input.
        /// </summary>
        /// <param name="donations">Donations to validate.</param>
        /// <returns>Whether donations are valid.</returns>
        public bool Validate(decimal donations)
        {
            if (donations < 0)
            {
                return false;
            }

            return true;
        }
    }
}
