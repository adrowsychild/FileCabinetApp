using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// Date of birth validator.
    /// </summary>
    public interface IDateOfBirthValidator
    {
        /// <summary>
        /// Validates date of birth of user's input.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth to validate.</param>
        /// <returns>Whether date of birth is valid.</returns>
        public bool Validate(DateTime dateOfBirth);
    }
}
