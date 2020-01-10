using System;
using System.Collections.Generic;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators.DefaultValidator
{
    /// <summary>
    /// Last name validator.
    /// </summary>
    public class DefaultLastNameValidator : ILastNameValidator
    {
        /// <summary>
        /// Validates last name of user's input.
        /// </summary>
        /// <param name="lastName">Last name to validate.</param>
        /// <returns>Whether last name is valid.</returns>
        public bool Validate(string lastName)
        {
            if (string.IsNullOrEmpty(lastName) || lastName.Length < 2 || lastName.Length > 60)
            {
                return false;
            }

            return true;
        }
    }
}
