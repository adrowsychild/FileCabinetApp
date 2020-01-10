using System;
using System.Collections.Generic;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators.CustomValidator
{
    /// <summary>
    /// Custom date of birth validator.
    /// </summary>
    public class CustomDateOfBirthValidator : IDateOfBirthValidator
    {
        /// <summary>
        /// Validates date of birth of user's input.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth to validate.</param>
        /// <returns>Whether date of birth is valid.</returns>
        public bool Validate(DateTime dateOfBirth)
        {
            if (dateOfBirth < new DateTime(1950, 1, 1) || dateOfBirth > DateTime.Now)
            {
                return false;
            }

            if ((DateTime.Today.Year - dateOfBirth.Year) < 18)
            {
                return false;
            }

            return true;
        }
    }
}
