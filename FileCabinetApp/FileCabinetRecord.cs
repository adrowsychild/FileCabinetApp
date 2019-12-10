namespace FileCabinetApp
{
    using System;

    public class FileCabinetRecord
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime DateOfBirth { get; set; }

        public short FavouriteNumber { get; set; }

        public char FavouriteCharacter { get; set; }

        public string FavouriteGame { get; set; }

        public decimal Donations { get; set; }
    }
}