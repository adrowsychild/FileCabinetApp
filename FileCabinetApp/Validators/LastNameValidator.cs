using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Last name validator.
    /// </summary>
    public class LastNameValidator : IRecordValidator
    {
        private int minLength;
        private int maxLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="LastNameValidator"/> class.
        /// </summary>
        /// <param name="min">Minimum possible length of first name.</param>
        /// <param name="max">Maximum possible length of last name.</param>
        public LastNameValidator(int min, int max)
        {
            this.minLength = min;
            this.maxLength = max;
        }

        /// <summary>
        /// Validates last name of user's input.
        /// </summary>
        /// <param name="record">Record to validate.</param>
        /// <returns>Exception message.</returns>
        public string Validate(FileCabinetRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException($"Record is invalid");
            }

            if (record.LastName.Length < this.minLength || record.LastName.Length > this.maxLength)
            {
                return "Last name is invalid. Should contain from " + this.minLength + " to " + this.maxLength + " characters.";
            }
            else
            {
                return null;
            }
        }
    }
}
