using System;
using System.Collections.Generic;
using System.Text;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Favourite character validator.
    /// </summary>
    public class FavouriteCharacterValidator : IRecordValidator
    {
        private int symbolCase;

        /// <summary>
        /// Initializes a new instance of the <see cref="FavouriteCharacterValidator"/> class.
        /// </summary>
        /// <param name="symbolCase">Case of character: 0 if case insensitive,
        /// -1 if only lowercase allowed, 1 if only uppercase allowed.</param>
        public FavouriteCharacterValidator(int symbolCase)
        {
            this.symbolCase = symbolCase;
        }

        /// <summary>
        /// Validates favourite character of user's input.
        /// </summary>
        /// <param name="record">Record to validate.</param>
        /// <returns>Whether favourite character is valid.</returns>
        public bool Validate(FileCabinetRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException($"Record is invalid.");
            }

            if (!char.IsLetter(record.FavouriteCharacter))
            {
                return false;
            }

            switch (this.symbolCase)
            {
                case -1:
                    if (char.IsUpper(record.FavouriteCharacter))
                    {
                        return false;
                    }

                    break;

                case 1:
                    if (char.IsLower(record.FavouriteCharacter))
                    {
                        return false;
                    }

                    break;
            }

            return true;
        }
    }
}
