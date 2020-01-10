using System;
using System.Collections.Generic;
using FileCabinetApp.Validators.CustomValidator;

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
        /// <returns>Exception message.</returns>
        public string ValidateParameters(FileCabinetRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException($"{record} object is invalid.");
            }

            if (!new CustomFirstNameValidator().Validate(record.FirstName))
            {
                return "First name is invalid.";
            }

            if (!new CustomLastNameValidator().Validate(record.LastName))
            {
                return "Last name is invalid.";
            }

            if (!new CustomDateOfBirthValidator().Validate(record.DateOfBirth))
            {
                return "Date of birth is invalid.";
            }

            if (!new CustomFavouriteNumberValidator().Validate(record.FavouriteNumber))
            {
                return "Favourite number is invalid.";
            }

            if (!new CustomFavouriteCharacterValidator().Validate(record.FavouriteCharacter))
            {
                return "Favourite character is invalid.";
            }

            if (!new CustomFavouriteGameValidator().Validate(record.FavouriteGame))
            {
                return "Favourite game is invalid.";
            }

            if (!new CustomDonationsValidator().Validate(record.Donations))
            {
                return "Donations are invalid.";
            }

            return null;
        }
    }
}
