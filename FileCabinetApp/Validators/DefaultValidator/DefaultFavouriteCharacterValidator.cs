using System;
using System.Collections.Generic;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators.DefaultValidator
{
    /// <summary>
    /// Favourite character validator.
    /// </summary>
    public class DefaultFavouriteCharacterValidator : IFavouriteCharacterValidator
    {
        /// <summary>
        /// Validates favourite character of user's input.
        /// </summary>
        /// <param name="favCharacter">Favourite character to validate.</param>
        /// <returns>Whether favourite character is valid.</returns>
        public bool Validate(char favCharacter)
        {
            if (favCharacter < 65 || (favCharacter > 90 && favCharacter < 97) || favCharacter > 122)
            {
                return false;
            }

            return true;
        }
    }
}
