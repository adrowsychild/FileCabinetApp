using System;
using System.Collections.Generic;

namespace FileCabinetApp
{
    /// <summary>
    /// Validator for user's input.
    /// </summary>
    public interface IRecordValidator
    {
        /// <summary>
        /// Validates the user's input.
        /// </summary>
        /// <param name="record">Record to validate.</param>
        /// <returns>Exception message.</returns>
        string ValidateParameters(FileCabinetRecord record);

        /// <summary>
        /// Validates first name of user's input.
        /// </summary>
        /// <param name="firstName">First name to validate.</param>
        /// <returns>Whether first name is valid.</returns>
        public bool ValidateFirstName(string firstName);

        /// <summary>
        /// Validates last name of user's input.
        /// </summary>
        /// <param name="lastName">Last name to validate.</param>
        /// <returns>Whether last name is valid.</returns>
        public bool ValidateLastName(string lastName);

        /// <summary>
        /// Validates date of birth of user's input.
        /// </summary>
        /// <param name="dateOfBirth">Date of birth to validate.</param>
        /// <returns>Whether date of birth is valid.</returns>
        public bool ValidateDateOfBirth(DateTime dateOfBirth);

        /// <summary>
        /// Validates favourite number of user's input.
        /// </summary>
        /// <param name="favouriteNumber">Favourite number to validate.</param>
        /// <returns>Whether favourite number is valid.</returns>
        public bool ValidateFavouriteNumber(short favouriteNumber);

        /// <summary>
        /// Validates favourite character of user's input.
        /// </summary>
        /// <param name="favouriteCharacter">Favourite character to validate.</param>
        /// <returns>Whether favourite character is valid.</returns>
        public bool ValidateFavouriteCharacter(char favouriteCharacter);

        /// <summary>
        /// Validates favourite game of user's input.
        /// </summary>
        /// <param name="favouriteGame">Favourite game to validate.</param>
        /// <returns>Whether favourite game is valid.</returns>
        public bool ValidateFavouriteGame(string favouriteGame);

        /// <summary>
        /// Validates donations of user's input.
        /// </summary>
        /// <param name="donations">Donations to validate.</param>
        /// <returns>Whether donations are valid.</returns>
        public bool ValidateDonations(decimal donations);
    }
}
