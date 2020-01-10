using System;
using System.Collections.Generic;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators.CustomValidator
{
    /// <summary>
    /// Custom favourite number validator.
    /// </summary>
    public class CustomFavouriteNumberValidator : IFavouriteNumberValidator
    {
        /// <summary>
        /// Validates favourite number of user's input.
        /// </summary>
        /// <param name="favNumber">Favourite number to validate.</param>
        /// <returns>Whether favourite number is valid.</returns>
        public bool Validate(short favNumber)
        {
            if (favNumber < 0)
            {
                return false;
            }

            return true;
        }
    }
}
