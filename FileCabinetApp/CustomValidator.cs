using System;
using System.Collections.Generic;
using System.Text;

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
        public void ValidateParameters(FileCabinetRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException($"{record} object is invalid.");
            }

            if (string.IsNullOrEmpty(record.FirstName) || record.FirstName.Length < 2 || record.FirstName.Length > 60)
            {
                throw new ArgumentException("First name is invalid.");
            }

            if (record.FirstName[0] < 65 || record.FirstName[0] > 90)
            {
                throw new ArgumentException("First name is invalid. Should be capitalized.");
            }

            if (string.IsNullOrEmpty(record.LastName) || record.LastName.Length < 2 || record.LastName.Length > 60)
            {
                throw new ArgumentException("Last name is invalid.");
            }

            if (record.LastName[0] < 65 || record.LastName[0] > 90)
            {
                throw new ArgumentException("Last name is invalid. Should be capitalized.");
            }

            if (record.DateOfBirth < new DateTime(1950, 1, 1))
            {
                throw new ArgumentException("Date of birth is invalid.");
            }

            if ((DateTime.Today.Year - record.DateOfBirth.Year) < 18)
            {
                throw new ArgumentException("Date of birth is invalid. You should be at least 18 years old.");
            }

            if (record.FavouriteNumber < 0)
            {
                throw new ArgumentException("Favourite number is invalid.");
            }

            if (record.FavouriteCharacter < 65 || (record.FavouriteCharacter > 90 && record.FavouriteCharacter < 97) || record.FavouriteCharacter > 122)
            {
                throw new ArgumentException("FavouriteCharacter is invalid.");
            }

            if (string.IsNullOrEmpty(record.FavouriteGame))
            {
                throw new ArgumentException("FavouriteGame is invalid.");
            }

            if (record.Donations < 0)
            {
                throw new ArgumentException("Donations is invalid.");
            }
        }
        }
}
