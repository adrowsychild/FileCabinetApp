namespace FileCabinetApp
{
    using System;
    using System.Collections.Generic;

    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth, short favouriteNumber, char favouriteCharacter, string favouriteGame, decimal donations)
        {
            this.CheckFields(firstName, lastName, dateOfBirth, favouriteNumber, favouriteCharacter, favouriteGame, donations);

            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
                FavouriteNumber = favouriteNumber,
                FavouriteCharacter = favouriteCharacter,
                FavouriteGame = favouriteGame,
                Donations = donations,
            };

            this.list.Add(record);

            return record.Id;
        }

        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, short favouriteNumber, char favouriteCharacter, string favouriteGame, decimal donations)
        {
            if (id < 0 || id > this.GetStat())
            {
                throw new ArgumentException(nameof(id) + " is invalid.");
            }

            this.CheckFields(firstName, lastName, dateOfBirth, favouriteNumber, favouriteCharacter, favouriteGame, donations);

            this.list[id].FirstName = firstName;
            this.list[id].LastName = lastName;
            this.list[id].DateOfBirth = dateOfBirth;
            this.list[id].FavouriteNumber = favouriteNumber;
            this.list[id].FavouriteCharacter = favouriteCharacter;
            this.list[id].FavouriteGame = favouriteGame;
            this.list[id].Donations = donations;
        }

        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            List<FileCabinetRecord> foundRecords = new List<FileCabinetRecord>();

            for (int i = 0; i < this.list.Count; i++)
            {
                if (firstName.ToLower() == this.list[i].FirstName.ToLower())
                {
                    foundRecords.Add(this.list[i]);
                }
            }

            return foundRecords.ToArray();
        }

        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            List<FileCabinetRecord> foundRecords = new List<FileCabinetRecord>();

            for (int i = 0; i < this.list.Count; i++)
            {
                if (lastName.ToLower() == this.list[i].LastName.ToLower())
                {
                    foundRecords.Add(this.list[i]);
                }
            }

            return foundRecords.ToArray();
        }

        public FileCabinetRecord[] GetRecords()
        {
            FileCabinetRecord[] listCopied = new FileCabinetRecord[this.list.Count];
            this.list.CopyTo(listCopied);
            return listCopied;
        }

        public int GetStat()
        {
            return this.list.Count;
        }

        private void CheckFields(string firstName, string lastName, DateTime dateOfBirth, short favouriteNumber, char favouriteCharacter, string favouriteGame, decimal donations)
        {
            if (string.IsNullOrEmpty(firstName) || firstName.Length < 2 || firstName.Length > 60)
            {
                throw new ArgumentException(nameof(firstName) + " is invalid.");
            }

            if (string.IsNullOrEmpty(lastName) || lastName.Length < 2 || lastName.Length > 60)
            {
                throw new ArgumentException(nameof(lastName) + " is invalid.");
            }

            if (dateOfBirth < new DateTime(1950, 1, 1) || dateOfBirth > DateTime.Now)
            {
                throw new ArgumentException(nameof(dateOfBirth) + " is invalid.");
            }

            if (favouriteNumber < 0)
            {
                throw new ArgumentException(nameof(favouriteNumber) + " is invalid.");
            }

            if (favouriteCharacter < 65 || (favouriteCharacter > 90 && favouriteCharacter < 97) || favouriteCharacter > 122)
            {
                throw new ArgumentException(nameof(favouriteCharacter) + " is invalid.");
            }

            if (string.IsNullOrEmpty(favouriteGame))
            {
                throw new ArgumentException(nameof(favouriteGame) + " is invalid.");
            }

            if (donations < 0)
            {
                throw new ArgumentException(nameof(donations) + " is invalid.");
            }
        }
    }
}