﻿namespace FileCabinetApp
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>();


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

            string firstname = firstName.ToLower();
            string lastname = lastName.ToLower();
            string dateofbirth = dateOfBirth.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture).ToLower();

            // firstname dictionary update
            if (!this.firstNameDictionary.ContainsKey(firstname))
            {
                List<FileCabinetRecord> listOfFirstNames = new List<FileCabinetRecord>();
                listOfFirstNames.Add(record);
                this.firstNameDictionary.Add(firstname, listOfFirstNames);
            }
            else
            {
                this.firstNameDictionary[firstname].Add(record);
            }

            // lastname dictionary update
            if (!this.lastNameDictionary.ContainsKey(lastname))
            {
                List<FileCabinetRecord> listOfLastNames = new List<FileCabinetRecord>();
                listOfLastNames.Add(record);
                this.lastNameDictionary.Add(lastname, listOfLastNames);
            }
            else
            {
                this.lastNameDictionary[lastname].Add(record);
            }

            // dateofbirth dictionary update
            if (!this.dateOfBirthDictionary.ContainsKey(dateofbirth))
            {
                List<FileCabinetRecord> listOfDatesOfBirth = new List<FileCabinetRecord>();
                listOfDatesOfBirth.Add(record);
                this.dateOfBirthDictionary.Add(dateofbirth, listOfDatesOfBirth);
            }
            else
            {
                this.dateOfBirthDictionary[dateofbirth].Add(record);
            }

            return record.Id;
        }

        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, short favouriteNumber, char favouriteCharacter, string favouriteGame, decimal donations)
        {
            if (id < 0 || id > this.GetStat())
            {
                throw new ArgumentException(nameof(id) + " is invalid.");
            }

            this.CheckFields(firstName, lastName, dateOfBirth, favouriteNumber, favouriteCharacter, favouriteGame, donations);

            string firstname = firstName.ToLower();
            string lastname = lastName.ToLower();
            string dateofbirth = dateOfBirth.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture).ToLower();

            this.firstNameDictionary[this.list[id].FirstName.ToLower()].Remove(this.list[id]);
            this.lastNameDictionary[this.list[id].LastName.ToLower()].Remove(this.list[id]);
            this.dateOfBirthDictionary[this.list[id].DateOfBirth.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture).ToLower()].Remove(this.list[id]);

            this.list[id].FirstName = firstName;
            this.list[id].LastName = lastName;
            this.list[id].DateOfBirth = dateOfBirth;
            this.list[id].FavouriteNumber = favouriteNumber;
            this.list[id].FavouriteCharacter = favouriteCharacter;
            this.list[id].FavouriteGame = favouriteGame;
            this.list[id].Donations = donations;

            // firstname dictionary update
            if (!this.firstNameDictionary.ContainsKey(firstname))
            {
                List<FileCabinetRecord> listOfFirstNames = new List<FileCabinetRecord>();
                listOfFirstNames.Add(this.list[id]);
                this.firstNameDictionary.Add(firstname, listOfFirstNames);
            }
            else
            {
                this.firstNameDictionary[firstname].Add(this.list[id]);
            }

            // lastname dictionary update
            if (!this.lastNameDictionary.ContainsKey(lastname))
            {
                List<FileCabinetRecord> listOfLastNames = new List<FileCabinetRecord>();
                listOfLastNames.Add(this.list[id]);
                this.lastNameDictionary.Add(lastname, listOfLastNames);
            }
            else
            {
                this.lastNameDictionary[lastname].Add(this.list[id]);
            }

            // dateofbirth dictionary update
            if (!this.dateOfBirthDictionary.ContainsKey(dateofbirth))
            {
                List<FileCabinetRecord> listOfDatesOfBirth = new List<FileCabinetRecord>();
                listOfDatesOfBirth.Add(this.list[id]);
                this.dateOfBirthDictionary.Add(dateofbirth, listOfDatesOfBirth);
            }
            else
            {
                this.dateOfBirthDictionary[dateofbirth].Add(this.list[id]);
            }
        }

        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            List<FileCabinetRecord> foundRecords = new List<FileCabinetRecord>();

            if (firstName == null)
            {
                throw new ArgumentException(nameof(firstName) + "is null.");
            }

            if (this.firstNameDictionary.ContainsKey(firstName.ToLower()))
            {
                foundRecords = this.firstNameDictionary[firstName.ToLower()];
                return foundRecords.ToArray();
            }
            else
            {
                return null;
            }
        }

        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            List<FileCabinetRecord> foundRecords = new List<FileCabinetRecord>();

            if (lastName == null)
            {
                throw new ArgumentException(nameof(lastName) + "is null.");
            }

            if (this.lastNameDictionary.ContainsKey(lastName.ToLower()))
            {
                foundRecords = this.lastNameDictionary[lastName.ToLower()];
                return foundRecords.ToArray();
            }
            else
            {
                return null;
            }
        }

        public FileCabinetRecord[] FindByDateOfBirth(string dateOfBirth)
        {
            List<FileCabinetRecord> foundRecords = new List<FileCabinetRecord>();

            if (dateOfBirth == null)
            {
                throw new ArgumentException(nameof(dateOfBirth) + "is null.");
            }

            if (this.dateOfBirthDictionary.ContainsKey(dateOfBirth.ToLower()))
            {
                foundRecords = this.dateOfBirthDictionary[dateOfBirth.ToLower()];
                return foundRecords.ToArray();
            }
            else
            {
                return null;
            }
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