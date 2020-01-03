using System;
using System.Collections.Generic;
using System.Text;

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
        void ValidateParameters(FileCabinetRecord record);
    }
}
