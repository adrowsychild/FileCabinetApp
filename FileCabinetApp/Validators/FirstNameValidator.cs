using System;
using System.Collections.Generic;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// First name validator.
    /// </summary>
    public class FirstNameValidator : IRecordValidator
    {
        private readonly int minLength;
        private readonly int maxLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="FirstNameValidator"/> class.
        /// </summary>
        /// <param name="min">Minimum possible length of first name.</param>
        /// <param name="max">Maximum possible length of last name.</param>
        public FirstNameValidator(int min, int max)
        {
            this.minLength = min;
            this.maxLength = max;
        }

        /// <summary>
        /// Validates first name of user's input.
        /// </summary>
        /// <param name="record">Record to validate.</param>
        /// <returns>Whether first name is valid.</returns>
        public bool Validate(FileCabinetRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException($"Record is invalid");
            }

            if (record.FirstName.Length < this.minLength || record.FirstName.Length > this.maxLength)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
