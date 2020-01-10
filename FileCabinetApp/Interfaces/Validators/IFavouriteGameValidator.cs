using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// Favourite game validator.
    /// </summary>
    public interface IFavouriteGameValidator
    {
        /// <summary>
        /// Validates favourite game of user's input.
        /// </summary>
        /// <param name="favGame">Favourite game to validate.</param>
        /// <returns>Whether favourite game is valid.</returns>
        public bool Validate(string favGame);
    }
}
