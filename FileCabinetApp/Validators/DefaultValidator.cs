using System;
using System.Collections.Generic;
using System.Text;

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

            if (!this.ValidateFirstName(record.FirstName))
            {
                return "First name is invalid.";
            }

            if (!this.ValidateLastName(record.LastName))
            {
                return "Last name is invalid.";
            }

            if (!this.ValidateDateOfBirth(record.DateOfBirth))
            {
                return "Date of birth is invalid.";
            }

            if (!this.ValidateFavouriteNumber(record.FavouriteNumber))
            {
                return "Favourite number is invalid.";
            }

            if (!this.ValidateFavouriteCharacter(record.FavouriteCharacter))
            {
                return "Favourite character is invalid.";
            }

            if (!this.ValidateFavouriteGame(record.FavouriteGame))
            {
                return "Favourite game is invalid.";
            }

            if (!this.ValidateDonations(record.Donations))
            {
                return "Donations are invalid.";
            }

            return null;
        }

        /// <summary>
        /// Validates first name of user's input.
        /// </summary>
        /// <param name="firstName">First name to validate.</param>
        /// <returns>Whether first name is valid.</returns>
        public bool ValidateFirstName(string firstName)
        {
            if (string.IsNullOrEmpty(firstName) || firstName.Length < 2 || firstName.Length > 60)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates last name of user's input.
        /// </summary>
        /// <param name="lastName">Last name to validate.</param>
        /// <returns>Whether last name is valid.</returns>
        public bool ValidateLastName(string lastName)
        {
            if (string.IsNullOrEmpty(lastName) || lastName.Length < 2 || lastName.Length > 60)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates date of birth of user's input.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth to validate.</param>
        /// <returns>Whether date of birth is valid.</returns>
        public bool ValidateDateOfBirth(DateTime dateOfBirth)
        {
            if (dateOfBirth < new DateTime(1950, 1, 1) || dateOfBirth > DateTime.Now)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates favourite number of user's input.
        /// </summary>
        /// <param name="favouriteNumber">Favourite number to validate.</param>
        /// <returns>Whether favourite number is valid.</returns>
        public bool ValidateFavouriteNumber(short favouriteNumber)
        {
            if (favouriteNumber < 0)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates favourite character of user's input.
        /// </summary>
        /// <param name="favouriteCharacter">Favourite character to validate.</param>
        /// <returns>Whether favourite character is valid.</returns>
        public bool ValidateFavouriteCharacter(char favouriteCharacter)
        {
            if (favouriteCharacter < 65 || (favouriteCharacter > 90 && favouriteCharacter < 97) || favouriteCharacter > 122)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates favourite game of user's input.
        /// </summary>
        /// <param name="favouriteGame">Favourite game to validate.</param>
        /// <returns>Whether favourite game is valid.</returns>
        public bool ValidateFavouriteGame(string favouriteGame)
        {
            if (string.IsNullOrEmpty(favouriteGame))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Validates donations of user's input.
        /// </summary>
        /// <param name="donations">Donations to validate.</param>
        /// <returns>Whether donations are valid.</returns>
        public bool ValidateDonations(decimal donations)
        {
            if (donations < 0)
            {
                return false;
            }

            return true;
        }
    }
}
