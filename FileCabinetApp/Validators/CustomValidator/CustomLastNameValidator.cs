using System;
using System.Collections.Generic;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators.CustomValidator
{
    /// <summary>
    /// Custom last name validator.
    /// </summary>
    public class CustomLastNameValidator : ILastNameValidator
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

            if (lastName[0] < 65 || lastName[0] > 90)
            {
                return false;
            }

            return true;
        }
    }
}
