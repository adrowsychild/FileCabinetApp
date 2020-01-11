using System;
using System.Collections.Generic;
using FileCabinetApp.Validators;

namespace FileCabinetApp
{
    /// <summary>
    /// Custom validator for user's input.
    /// </summary>
    public class CustomValidator : IRecordValidator
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

            if (!new FirstNameValidator(2, 15).Validate(record))
            {
                // return "First name is invalid.";
                return false;
            }

            if (!new LastNameValidator(2, 15).Validate(record))
            {
                // return "Last name is invalid.";
                return false;
            }

            if (!new DateOfBirthValidator(18, 65).Validate(record))
            {
                // return "Date of birth is invalid.";
                return false;
            }

            if (!new FavouriteNumberValidator(0, 10).Validate(record))
            {
                // return "Favourite number is invalid.";
                return false;
            }

            if (!new FavouriteCharacterValidator(-1).Validate(record))
            {
                // return "Favourite character is invalid.";
                return false;
            }

            if (!new FavouriteGameValidator(1, 15).Validate(record))
            {
                // return "Favourite game is invalid.";
                return false;
            }

            if (!new DonationsValidator(0.5m).Validate(record))
            {
                // return "Donations are invalid.";
                return false;
            }

            return true;
        }
    }
}
