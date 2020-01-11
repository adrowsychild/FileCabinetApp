using System;
using System.Collections.Generic;
using System.Text;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Donations validator.
    /// </summary>
    public class DonationsValidator : IRecordValidator
    {
        private readonly decimal minValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="DonationsValidator"/> class.
        /// </summary>
        /// <param name="minValue">Minimum possible value.</param>
        public DonationsValidator(decimal minValue)
        {
            this.minValue = minValue;
        }

        /// <summary>
        /// Validates donations of user's input.
        /// </summary>
        /// <param name="record">Record to validate.</param>
        /// <returns>Whether donations are valid.</returns>
        public bool Validate(FileCabinetRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException($"Record is invalid.");
            }

            if (record.Donations < this.minValue)
            {
                return false;
            }

            return true;
        }
    }
}
