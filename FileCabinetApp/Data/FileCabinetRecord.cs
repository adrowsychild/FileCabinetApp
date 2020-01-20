namespace FileCabinetApp
{
    using System;
    using System.Xml.Serialization;

    /// <summary>
    /// Class representing a record with user's info.
    /// </summary>
    [Serializable]
    public class FileCabinetRecord : ICloneable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecord"/> class.
        /// </summary>
        /// <param name="id">User's id in the user's list.</param>
        /// <param name="firstName">User's first name.</param>
        /// <param name="lastName">User's last name.</param>
        /// <param name="dateOfBirth">User's date of birth.</param>
        /// <param name="favouriteNumber">User's favourite number.</param>
        /// <param name="favouriteCharacter">User's favourite character.</param>
        /// <param name="favouriteGame">User's favourite game.</param>
        /// <param name="donations">User's donations.</param>
        public FileCabinetRecord(int id, string firstName, string lastName, DateTime dateOfBirth, short favouriteNumber, char favouriteCharacter, string favouriteGame, decimal donations)
        {
            this.Id = id;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.DateOfBirth = dateOfBirth;
            this.FavouriteNumber = favouriteNumber;
            this.FavouriteCharacter = favouriteCharacter;
            this.FavouriteGame = favouriteGame;
            this.Donations = donations;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecord"/> class.
        /// Unsafe constructor for using initializators.
        /// </summary>
        public FileCabinetRecord()
        {
        }

        /// <summary>
        /// Gets or sets user's id in the users' list.
        /// </summary>
        /// <value> Given id. </value>
        [XmlAttribute]
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets user's id in the users' first name.
        /// </summary>
        /// <value> Given first name. </value>
        [XmlElement]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets user's id in the users' last name.
        /// </summary>
        /// <value> Given last name. </value>
        [XmlElement]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets user's id in the users' date of birth.
        /// </summary>
        /// <value> Given date of birth. </value>
        [XmlElement]
        public DateTime DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets user's id in the users' favourite number.
        /// </summary>
        /// <value> Given favourite number. </value>
        [XmlElement]
        public short FavouriteNumber { get; set; }

        /// <summary>
        /// Gets or sets user's id in the users' favourite character.
        /// </summary>
        /// <value> Given favourite character. </value>
        [XmlElement]
        public char FavouriteCharacter { get; set; }

        /// <summary>
        /// Gets or sets user's id in the users' favourite game.
        /// </summary>
        /// <value> Given favourite game. </value>
        [XmlElement]
        public string FavouriteGame { get; set; }

        /// <summary>
        /// Gets or sets user's id in the users' donations.
        /// </summary>
        /// <value> Given donations. </value>
        [XmlElement]
        public decimal Donations { get; set; }

        public object Clone()
        {
            return new FileCabinetRecord
            {
                Id = this.Id,
                FirstName = this.FirstName,
                LastName = this.LastName,
                DateOfBirth = this.DateOfBirth,
                FavouriteNumber = this.FavouriteNumber,
                FavouriteCharacter = this.FavouriteCharacter,
                FavouriteGame = this.FavouriteGame,
                Donations = this.Donations,
            };
        }
    }
}