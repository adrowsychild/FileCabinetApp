using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// Last name validator.
    /// </summary>
    public interface ILastNameValidator
    {
        /// <summary>
        /// Validates last name of user's input.
        /// </summary>
        /// <param name="lastName">Last name to validate.</param>
        /// <returns>Whether last name is valid.</returns>
        public bool Validate(string lastName);
    }
}
