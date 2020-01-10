using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// Favourite number validator.
    /// </summary>
    public interface IFavouriteNumberValidator
    {
        /// <summary>
        /// Validates favourite number of user's input.
        /// </summary>
        /// <param name="favNumber">Favourite number to validate.</param>
        /// <returns>Whether favourite number is valid.</returns>
        public bool Validate(short favNumber);
    }
}
