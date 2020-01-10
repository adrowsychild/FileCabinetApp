using System;
using System.Collections.Generic;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators.CustomValidator
{
    /// <summary>
    /// Custom first name validator.
    /// </summary>
    public class CustomFirstNameValidator : IFirstNameValidator
    {
        /// <summary>
        /// Validates first name of user's input.
        /// </summary>
        /// <param name="firstName">First name to validate.</param>
        /// <returns>Whether first name is valid.</returns>
        public bool Validate(string firstName)
        {
            if (string.IsNullOrEmpty(firstName) || firstName.Length < 2 || firstName.Length > 60)
            {
                return false;
            }

            if (firstName[0] < 65 || firstName[0] > 90)
            {
                return false;
            }

            return true;
        }
    }
}
