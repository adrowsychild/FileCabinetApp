using System;
using System.Collections.Generic;
using System.Text;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators.DefaultValidator
{
    /// <summary>
    /// First name validator.
    /// </summary>
    public class DefaultFirstNameValidator : IFirstNameValidator
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

            return true;
        }
    }
}
