using System;
using System.Collections.Generic;
using System.Text;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Validators.DefaultValidator
{
    /// <summary>
    /// Favourite number validator.
    /// </summary>
    public class DefaultFavouriteNumberValidator : IFavouriteNumberValidator
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
