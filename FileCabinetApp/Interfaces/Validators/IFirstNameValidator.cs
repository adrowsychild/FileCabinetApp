using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// First name validator.
    /// </summary>
    public interface IFirstNameValidator
    {
        /// <summary>
        /// Validates first name of user's input.
        /// </summary>
        /// <param name="firstName">First name to validate.</param>
        /// <returns>Whether first name is valid.</returns>
        public bool Validate(string firstName);
    }
}
