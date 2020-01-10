using System;
using System.Collections.Generic;
using FileCabinetApp.Validators.DefaultValidator;

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
        /// <returns>Exception message.</returns>
        public string ValidateParameters(FileCabinetRecord record)
        {
            if (record == null)
            {
                return $"{record} object is invalid.";
            }

            if (!new DefaultFirstNameValidator().Validate(record.FirstName))
            {
                return "First name is invalid.";
            }

            if (!new DefaultLastNameValidator().Validate(record.LastName))
            {
                return "Last name is invalid.";
            }

            if (!new DefaultDateOfBirthValidator().Validate(record.DateOfBirth))
            {
                return "Date of birth is invalid.";
            }

            if (!new DefaultFavouriteNumberValidator().Validate(record.FavouriteNumber))
            {
                return "Favourite number is invalid.";
            }

            if (!new DefaultFavouriteCharacterValidator().Validate(record.FavouriteCharacter))
            {
                return "Favourite character is invalid.";
            }

            if (!new DefaultFavouriteGameValidator().Validate(record.FavouriteGame))
            {
                return "Favourite game is invalid.";
            }

            if (!new DefaultDonationsValidator().Validate(record.Donations))
            {
                return "Donations are invalid.";
            }

            return null;
        }
    }
}
