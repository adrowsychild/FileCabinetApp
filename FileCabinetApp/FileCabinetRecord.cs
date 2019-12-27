namespace FileCabinetApp
{
    using System;

    /// <summary>
    /// Class representing a record with user's info.
    /// </summary>
    public class FileCabinetRecord
    {
        /// <summary>
        /// Gets or sets user's id in the users' list.
        /// </summary>
        /// <value> Given id. </value>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets user's id in the users' first name.
        /// </summary>
        /// <value> Given first name. </value>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets user's id in the users' last name.
        /// </summary>
        /// <value> Given last name. </value>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets user's id in the users' date of birth.
        /// </summary>
        /// <value> Given date of birth. </value>
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets user's id in the users' favourite number.
        /// </summary>
        /// <value> Given favourite number. </value>
        public short FavouriteNumber { get; set; }

        /// <summary>
        /// Gets or sets user's id in the users' favourite character.
        /// </summary>
        /// <value> Given favourite character. </value>
        public char FavouriteCharacter { get; set; }

        /// <summary>
        /// Gets or sets user's id in the users' favourite game.
        /// </summary>
        /// <value> Given favourite game. </value>
        public string FavouriteGame { get; set; }

        /// <summary>
        /// Gets or sets user's id in the users' donations.
        /// </summary>
        /// <value> Given donations. </value>
        public decimal Donations { get; set; }
    }
}