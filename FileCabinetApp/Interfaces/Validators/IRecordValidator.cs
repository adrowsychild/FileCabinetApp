using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// Validator for user's input.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Validates the user's input.
        /// </summary>
        /// <param name="record">Record to validate.</param>
        /// <returns>The result of validation.</returns>
        bool Validate(FileCabinetRecord record);
    }
}
