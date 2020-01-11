using System;
using System.Collections.Generic;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Favourite game validator.
    /// </summary>
    public class FavouriteGameValidator : IRecordValidator
    {
        private readonly int minLength;
        private readonly int maxLength;

        /// <summary>
        /// Initializes a new instance of the <see cref="FavouriteGameValidator"/> class.
        /// </summary>
        /// <param name="min">Minimum possible length of name of the game.</param>
        /// <param name="max">Maximum possible length of name of the game.</param>
        public FavouriteGameValidator(int min, int max)
        {
            this.minLength = min;
            this.maxLength = max;
        }

        /// <summary>
        /// Validates favourite game of user's input.
        /// </summary>
        /// <param name="record">Record to validate.</param>
        /// <returns>Exception message.</returns>
        public string Validate(FileCabinetRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException($"Record is invalid.");
            }

            if (string.IsNullOrEmpty(record.FavouriteGame) || record.FavouriteGame.Length < this.minLength || record.FavouriteGame.Length > this.maxLength)
            {
                return "Favourite game is invalid. Should contain from " + this.minLength + " to " + this.maxLength + " characters.";
            }

            return null;
        }
    }
}
