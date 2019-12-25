namespace FileCabinetApp
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
            CheckFields(firstName, lastName, dateOfBirth, favouriteNumber, favouriteCharacter, favouriteGame, donations);

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

            UpdateDictionary(record, this.firstNameDictionary, firstName);

            UpdateDictionary(record, this.lastNameDictionary, lastName);

            UpdateDictionary(record, this.dateOfBirthDictionary, dateOfBirth.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture));

            return record.Id;
        }

        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, short favouriteNumber, char favouriteCharacter, string favouriteGame, decimal donations)
        {
            if (id < 0 || id > this.GetStat())
            {
                throw new ArgumentException(nameof(id) + " is invalid.");
            }

            CheckFields(firstName, lastName, dateOfBirth, favouriteNumber, favouriteCharacter, favouriteGame, donations);

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

            UpdateDictionary(this.list[id], this.firstNameDictionary, firstName);

            UpdateDictionary(this.list[id], this.lastNameDictionary, lastName);

            UpdateDictionary(this.list[id], this.dateOfBirthDictionary, dateOfBirth.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture));
        }

        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            FileCabinetRecord[] foundRecords = FindByKey(firstName, this.firstNameDictionary);
            return foundRecords;
        }

        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            FileCabinetRecord[] foundRecords = FindByKey(lastName, this.lastNameDictionary);
            return foundRecords;
        }

        public FileCabinetRecord[] FindByDateOfBirth(string dateOfBirth)
        {
            FileCabinetRecord[] foundRecords = FindByKey(dateOfBirth, this.dateOfBirthDictionary);
            return foundRecords;
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

        private static void UpdateDictionary(FileCabinetRecord record, Dictionary<string, List<FileCabinetRecord>> dictionary, string key)
        {
            string keyLowered = key.ToLower();
            if (!dictionary.ContainsKey(keyLowered))
            {
                List<FileCabinetRecord> listOfProperties = new List<FileCabinetRecord>
                {
                    record,
                };

                dictionary.Add(keyLowered, listOfProperties);
            }
            else
            {
                dictionary[keyLowered].Add(record);
            }
        }

        private static FileCabinetRecord[] FindByKey(string key, Dictionary<string, List<FileCabinetRecord>> dictionary)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Argument to find by is invalid.");
            }

            if (dictionary.ContainsKey(key.ToLower()))
            {
                List<FileCabinetRecord> foundRecords = dictionary[key.ToLower()];
                return foundRecords.ToArray();
            }
            else
            {
                throw new ArgumentException("Records not found.");
            }
        }

        private static void CheckFields(string firstName, string lastName, DateTime dateOfBirth, short favouriteNumber, char favouriteCharacter, string favouriteGame, decimal donations)
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