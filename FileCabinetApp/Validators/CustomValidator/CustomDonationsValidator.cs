using System;
using System.Collections.Generic;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators.CustomValidator
{
    /// <summary>
    /// Custom donations validator.
    /// </summary>
    public class CustomDonationsValidator : IDonationsValidator
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
