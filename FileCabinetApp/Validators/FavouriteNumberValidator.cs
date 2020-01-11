using System;
using System.Collections.Generic;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Favourite number validator.
    /// </summary>
    public class FavouriteNumberValidator : IRecordValidator
    {
        private short minValue;
        private short maxValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="FavouriteNumberValidator"/> class.
        /// </summary>
        /// <param name="minValue">Minimum possible value.</param>
        /// <param name="maxValue">Maximum possible value.</param>
        public FavouriteNumberValidator(short minValue, short maxValue)
        {
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        /// <summary>
        /// Validates favourite number of user's input.
        /// </summary>
        /// <param name="record">Record to validate.</param>
        /// <returns>Whether favourite number is valid.</returns>
        public bool Validate(FileCabinetRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException($"Record is invalid.");
            }

            if (record.FavouriteNumber < this.minValue || record.FavouriteNumber > this.maxValue)
            {
                return false;
            }

            return true;
        }
    }
}
