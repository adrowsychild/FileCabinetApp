using System;
using System.Collections.Generic;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators.DefaultValidator
{
    /// <summary>
    /// Favourite game validator.
    /// </summary>
    public class DefaultFavouriteGameValidator : IFavouriteGameValidator
    {
        /// <summary>
        /// Validates favourite game of user's input.
        /// </summary>
        /// <param name="favGame">Favourite game to validate.</param>
        /// <returns>Whether favourite game is valid.</returns>
        public bool Validate(string favGame)
        {
            if (string.IsNullOrEmpty(favGame))
            {
                return false;
            }

            return true;
        }
    }
}
