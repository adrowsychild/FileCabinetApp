using System;
using System.Collections.Generic;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// Default validator for user's input.
    /// </summary>
    public class DefaultValidator : IRecordValidator
    {
        /// <summary>
        /// Validates the user's input.
        /// </summary>
        /// <param name="record">Record to validate.</param>
        /// <returns>Whether record is valid.</returns>
        public bool Validate(FileCabinetRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException($"{record} object is invalid.");
            }

            if (!new FirstNameValidator(2, 60).Validate(record))
            {
                // return "First name is invalid.";
                return false;
            }

            if (!new LastNameValidator(2, 60).Validate(record))
            {
                // return "Last name is invalid.";
                return false;
            }

            if (!new DateOfBirthValidator(0, 100).Validate(record))
            {
                // return "Date of birth is invalid.";
                return false;
            }

            if (!new FavouriteNumberValidator(0, short.MaxValue).Validate(record))
            {
                // return "Favourite number is invalid.";
                return false;
            }

            if (!new FavouriteCharacterValidator(0).Validate(record))
            {
                // return "Favourite character is invalid.";
                return false;
            }

            if (!new FavouriteGameValidator(1, 60).Validate(record))
            {
                // return "Favourite game is invalid.";
                return false;
            }

            if (!new DonationsValidator(0).Validate(record))
            {
                // return "Donations are invalid.";
                return false;
            }

            return true;
        }
    }
}
