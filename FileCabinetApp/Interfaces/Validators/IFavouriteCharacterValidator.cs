using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// Favourite character validator.
    /// </summary>
    public interface IFavouriteCharacterValidator
    {
        /// <summary>
        /// Validates favourite character of user's input.
        /// </summary>
        /// <param name="favCharacter">Favourite character to validate.</param>
        /// <returns>Whether favourite character is valid.</returns>
        public bool Validate(char favCharacter);
    }
}