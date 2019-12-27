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
        /// <param name="record">User's info.</param>
        /// <returns>User's id in the users' list.</returns>
        public int CreateRecord(FileCabinetRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException($"{record} object is invalid.");
            }

            CheckFields(record);

            record.Id = this.list.Count + 1;

            this.list.Add(record);

            UpdateDictionary(record, this.firstNameDictionary, record.FirstName);

            UpdateDictionary(record, this.lastNameDictionary, record.LastName);

            UpdateDictionary(record, this.dateOfBirthDictionary, record.DateOfBirth.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture));

            return record.Id;
        }

        /// <summary>
        /// Edits the existing record.
        /// </summary>
        /// <param name="record">Record to edit.</param>
        public void EditRecord(FileCabinetRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException($"{record} object is invalid.");
            }

            if (record.Id < 0 || record.Id > this.GetStat())
            {
                throw new ArgumentException(message: $"{record.Id} is invalid", nameof(record));
            }

            CheckFields(record);

            this.firstNameDictionary[this.list[record.Id].FirstName.ToLower()].Remove(this.list[record.Id]);
            this.lastNameDictionary[this.list[record.Id].LastName.ToLower()].Remove(this.list[record.Id]);
            this.dateOfBirthDictionary[this.list[record.Id].DateOfBirth.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture).ToLower()].Remove(this.list[record.Id]);

            this.list[record.Id] = record;

            UpdateDictionary(this.list[record.Id], this.firstNameDictionary, record.FirstName);

            UpdateDictionary(this.list[record.Id], this.lastNameDictionary, record.LastName);

            UpdateDictionary(this.list[record.Id], this.dateOfBirthDictionary, record.DateOfBirth.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture));

            this.list[record.Id].Id++;
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
        /// <param name="record">User's info.</param>
        private static void CheckFields(FileCabinetRecord record)
        {
            if (string.IsNullOrEmpty(record.FirstName) || record.FirstName.Length < 2 || record.FirstName.Length > 60)
            {
                throw new ArgumentException(message: "First name is invalid.", nameof(record));
            }

            if (string.IsNullOrEmpty(record.LastName) || record.LastName.Length < 2 || record.LastName.Length > 60)
            {
                throw new ArgumentException(message: "Last name is invalid.", nameof(record));
            }

            if (record.DateOfBirth < new DateTime(1950, 1, 1) || record.DateOfBirth > DateTime.Now)
            {
                throw new ArgumentException(message: "Date of birth is invalid.", nameof(record));
            }

            if (record.FavouriteNumber < 0)
            {
                throw new ArgumentException(message: "Favourite number is invalid.", nameof(record));
            }

            if (record.FavouriteCharacter < 65 || (record.FavouriteCharacter > 90 && record.FavouriteCharacter < 97) || record.FavouriteCharacter > 122)
            {
                throw new ArgumentException(message: "FavouriteCharacter is invalid.", nameof(record));
            }

            if (string.IsNullOrEmpty(record.FavouriteGame))
            {
                throw new ArgumentException(message: "FavouriteGame is invalid.", nameof(record));
            }

            if (record.Donations < 0)
            {
                throw new ArgumentException(message: "Donations is invalid.", nameof(record));
            }
        }
    }
}