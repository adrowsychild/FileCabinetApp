namespace FileCabinetApp
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    /// <summary>
    /// Class for working with list of users.
    /// </summary>
    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        /// <summary>
        /// Creates a new record.
        /// </summary>
        /// <param name="firstName">User's first name.</param>
        /// <param name="lastName">User's last name.</param>
        /// <param name="dateOfBirth">User's date of birth.</param>
        /// <param name="favouriteNumber">User's favourite number.</param>
        /// <param name="favouriteCharacter">User's favourite character.</param>
        /// <param name="favouriteGame">User's favourite game.</param>
        /// <param name="donations">User's donations.</param>
        /// <returns>User's id in the users' list.</returns>
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

        /// <summary>
        /// Edits the new record by given id.
        /// </summary>
        /// <param name="id">User's id in the users' list.</param>
        /// <param name="firstName">User's first name.</param>
        /// <param name="lastName">User's last name.</param>
        /// <param name="dateOfBirth">User's date of birth.</param>
        /// <param name="favouriteNumber">User's favourite number.</param>
        /// <param name="favouriteCharacter">User's favourite character.</param>
        /// <param name="favouriteGame">User's favourite game.</param>
        /// <param name="donations">User's donations.</param>
        public void EditRecord(int id, string firstName, string lastName, DateTime dateOfBirth, short favouriteNumber, char favouriteCharacter, string favouriteGame, decimal donations)
        {
            if (id < 0 || id > this.GetStat())
            {
                throw new ArgumentException(message: $"{id} is invalid", nameof(id));
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

        /// <summary>
        /// Searches the records by first name.
        /// </summary>
        /// <param name="firstName">Given first name.</param>
        /// <returns>The array of records.</returns>
        public FileCabinetRecord[] FindByFirstName(string firstName)
        {
            FileCabinetRecord[] foundRecords = FindByKey(firstName, this.firstNameDictionary);
            return foundRecords;
        }

        /// <summary>
        /// Searches the records by last name.
        /// </summary>
        /// <param name="lastName">Given last name.</param>
        /// <returns>The array of records.</returns>
        public FileCabinetRecord[] FindByLastName(string lastName)
        {
            FileCabinetRecord[] foundRecords = FindByKey(lastName, this.lastNameDictionary);
            return foundRecords;
        }

        /// <summary>
        /// Searches the records by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">Given date of birth.</param>
        /// <returns>The array of records.</returns>
        public FileCabinetRecord[] FindByDateOfBirth(string dateOfBirth)
        {
            FileCabinetRecord[] foundRecords = FindByKey(dateOfBirth, this.dateOfBirthDictionary);
            return foundRecords;
        }

        /// <summary>
        /// Gets all the records.
        /// </summary>
        /// <returns>The array of records.</returns>
        public FileCabinetRecord[] GetRecords()
        {
            FileCabinetRecord[] listCopied = new FileCabinetRecord[this.list.Count];
            this.list.CopyTo(listCopied);
            return listCopied;
        }

        /// <summary>
        /// Returns the number of records in the list.
        /// </summary>
        /// <returns>The number of records.</returns>
        public int GetStat()
        {
            return this.list.Count;
        }

        /// <summary>
        /// Adds a new record to the dictionary by given key.
        /// </summary>
        /// <param name="record">The record to add.</param>
        /// <param name="dictionary">The dictionary to add to.</param>
        /// <param name="key">The key to add the record by.</param>
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

        /// <summary>
        /// Searches for the records in the dictionary by given key.
        /// </summary>
        /// <param name="key">The key to search by.</param>
        /// <param name="dictionary">The dictionary to search in.</param>
        /// <returns>The array of records.</returns>
        private static FileCabinetRecord[] FindByKey(string key, Dictionary<string, List<FileCabinetRecord>> dictionary)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException(message: $"{key} to find by is invalid", nameof(key));
            }

            if (dictionary.ContainsKey(key.ToLower()))
            {
                List<FileCabinetRecord> foundRecords = dictionary[key.ToLower()];
                return foundRecords.ToArray();
            }
            else
            {
                throw new ArgumentException(message: $"{key} not found", nameof(key));
            }
        }

        /// <summary>
        /// Validates the input fields of user's info.
        /// </summary>
        /// <param name="firstName">User's first name.</param>
        /// <param name="lastName">User's last name.</param>
        /// <param name="dateOfBirth">User's date of birth.</param>
        /// <param name="favouriteNumber">User's favourite number.</param>
        /// <param name="favouriteCharacter">User's favourite character.</param>
        /// <param name="favouriteGame">User's favourite game.</param>
        /// <param name="donations">User's donations.</param>
        private static void CheckFields(string firstName, string lastName, DateTime dateOfBirth, short favouriteNumber, char favouriteCharacter, string favouriteGame, decimal donations)
        {
            if (string.IsNullOrEmpty(firstName) || firstName.Length < 2 || firstName.Length > 60)
            {
                throw new ArgumentException(message: $"{firstName} is invalid.", nameof(firstName));
            }

            if (string.IsNullOrEmpty(lastName) || lastName.Length < 2 || lastName.Length > 60)
            {
                throw new ArgumentException(message: $"{lastName} is invalid.", nameof(lastName));
            }

            if (dateOfBirth < new DateTime(1950, 1, 1) || dateOfBirth > DateTime.Now)
            {
                throw new ArgumentException(message: $"{dateOfBirth} is invalid.", nameof(dateOfBirth));
            }

            if (favouriteNumber < 0)
            {
                throw new ArgumentException(message: $"{favouriteNumber} is invalid.", nameof(favouriteNumber));
            }

            if (favouriteCharacter < 65 || (favouriteCharacter > 90 && favouriteCharacter < 97) || favouriteCharacter > 122)
            {
                throw new ArgumentException(message: $"{favouriteCharacter} is invalid.", nameof(favouriteCharacter));
            }

            if (string.IsNullOrEmpty(favouriteGame))
            {
                throw new ArgumentException(message: $"{favouriteGame} is invalid.", nameof(favouriteGame));
            }

            if (donations < 0)
            {
                throw new ArgumentException(message: $"{donations} is invalid.", nameof(donations));
            }
        }
    }
}