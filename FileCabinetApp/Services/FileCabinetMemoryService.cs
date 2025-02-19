﻿namespace FileCabinetApp
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using FileCabinetApp.Interfaces;

    /// <summary>
    /// Class for working with list of users.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private const int Deleted = 0;
        private readonly IRecordValidator validator;

        private List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        private Dictionary<string, List<FileCabinetRecord>> idsDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private Dictionary<string, List<FileCabinetRecord>> favNumberDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private Dictionary<string, List<FileCabinetRecord>> favCharacterDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private Dictionary<string, List<FileCabinetRecord>> favGameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private Dictionary<string, List<FileCabinetRecord>> donationsDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        private Dictionary<string, IEnumerable<FileCabinetRecord>> idsCache = new Dictionary<string, IEnumerable<FileCabinetRecord>>();
        private Dictionary<string, IEnumerable<FileCabinetRecord>> firstNameCache = new Dictionary<string, IEnumerable<FileCabinetRecord>>();
        private Dictionary<string, IEnumerable<FileCabinetRecord>> lastNameCache = new Dictionary<string, IEnumerable<FileCabinetRecord>>();
        private Dictionary<string, IEnumerable<FileCabinetRecord>> dateOfBirthCache = new Dictionary<string, IEnumerable<FileCabinetRecord>>();
        private Dictionary<string, IEnumerable<FileCabinetRecord>> favNumberCache = new Dictionary<string, IEnumerable<FileCabinetRecord>>();
        private Dictionary<string, IEnumerable<FileCabinetRecord>> favCharCache = new Dictionary<string, IEnumerable<FileCabinetRecord>>();
        private Dictionary<string, IEnumerable<FileCabinetRecord>> favGameCache = new Dictionary<string, IEnumerable<FileCabinetRecord>>();
        private Dictionary<string, IEnumerable<FileCabinetRecord>> donationsCache = new Dictionary<string, IEnumerable<FileCabinetRecord>>();

        private List<int> ids = new List<int>() { 0 };

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetMemoryService"/> class.
        /// </summary>
        /// <param name="validator">Special validator.</param>
        public FileCabinetMemoryService(IRecordValidator validator)
        {
            this.validator = validator;
        }

        /// <summary>
        /// Creates a new record.
        /// </summary>
        /// <param name="record">User's info.</param>
        /// <returns>User's id in the users' list.</returns>
        public int CreateRecord(FileCabinetRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException($"Record object is invalid.");
            }

            record.Id = this.ids.Max() + 1;

            string exceptionMessage = this.validator.Validate(record);
            if (exceptionMessage != null)
            {
                return -1;
            }

            this.list.Add(record);
            this.UpdateDictionaries(record);
            this.PurgeCache(record);
            this.ids.Add(record.Id);

            return record.Id;
        }

        /// <summary>
        /// Adds record to the list of records.
        /// </summary>
        /// <param name="record">Record to add.</param>
        /// <returns>Record's id.</returns>
        public int AddRecord(FileCabinetRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException($"Record object is invalid.");
            }

            string exceptionMessage = this.validator.Validate(record);
            if (exceptionMessage != null)
            {
                return -1;
            }

            if (this.ids.Contains(record.Id))
            {
                int indexOfPrev = this.list.FindIndex(rec => rec.Id.Equals(record.Id));

                this.RemoveFromDictionary(indexOfPrev);

                this.list[indexOfPrev] = record;
            }
            else
            {
                this.list.Add(record);
                this.ids.Add(record.Id);
            }

            this.UpdateDictionaries(record);
            this.PurgeCache(record);

            return record.Id;
        }

        /// <summary>
        /// Edits the existing record.
        /// </summary>
        /// <param name="record">Record to edit.</param>
        /// <returns>Whether operation succeeded.</returns>
        public int EditRecord(FileCabinetRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException($"Record object is invalid.");
            }

            if (record.Id < 0 || !this.ids.Contains(record.Id))
            {
                return -1;
            }

            string exceptionMessage = this.validator.Validate(record);
            if (exceptionMessage != null)
            {
                return -1;
            }

            int indexOfPrev = this.list.FindIndex(rec => rec.Id.Equals(record.Id));

            this.RemoveFromDictionary(indexOfPrev);

            this.ids.Remove(this.list[indexOfPrev].Id);

            this.list[indexOfPrev] = record;
            this.UpdateDictionaries(record);
            this.PurgeCache(record);

            return 0;
        }

        /// <summary>
        /// Removes a record from the list by given id.
        /// </summary>
        /// <param name="id">Id to remove record by.</param>
        /// <returns>Id of removed record if succeeded, -1 otherwise.
        /// </returns>
        public int RemoveRecord(int id)
        {
            int indexToRemove = this.list.FindIndex(rec => rec.Id.Equals(id));
            if (indexToRemove == -1)
            {
                return -1;
            }

            this.PurgeCache(this.list[indexToRemove]);
            this.RemoveFromDictionary(indexToRemove);

            this.ids.Remove(this.list[indexToRemove].Id);

            this.list.Remove(this.list[indexToRemove]);

            return id;
        }

        /// <summary>
        /// Searches the records by id.
        /// </summary>
        /// <param name="id">Given id.</param>
        /// <returns>The array of records.</returns>
        public IEnumerable<FileCabinetRecord> FindById(string id)
        {
            return FindInCache(this.idsDictionary, this.idsCache, id);
        }

        /// <summary>
        /// Searches the records by first name.
        /// </summary>
        /// <param name="firstName">Given first name.</param>
        /// <returns>The array of records.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            return FindInCache(this.firstNameDictionary, this.firstNameCache, firstName);
        }

        /// <summary>
        /// Searches the records by last name.
        /// </summary>
        /// <param name="lastName">Given last name.</param>
        /// <returns>The array of records.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            return FindInCache(this.lastNameDictionary, this.lastNameCache, lastName);
        }

        /// <summary>
        /// Searches the records by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">Given date of birth.</param>
        /// <returns>The array of records.</returns>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            return FindInCache(this.dateOfBirthDictionary, this.dateOfBirthCache, dateOfBirth);
        }

        /// <summary>
        /// Searches the records by favourite number.
        /// </summary>
        /// <param name="favNumber">Given favourite number.</param>
        /// <returns>The array of records.</returns>
        public IEnumerable<FileCabinetRecord> FindByFavouriteNumber(string favNumber)
        {
            return FindInCache(this.favNumberDictionary, this.favNumberCache, favNumber);
        }

        /// <summary>
        /// Searches the records by favourite character.
        /// </summary>
        /// <param name="favChar">Given favourite character.</param>
        /// <returns>The array of records.</returns>
        public IEnumerable<FileCabinetRecord> FindByFavouriteCharacter(string favChar)
        {
            return FindInCache(this.favCharacterDictionary, this.favCharCache, favChar);
        }

        /// <summary>
        /// Searches the records by favourite game.
        /// </summary>
        /// <param name="favGame">Given favourite game.</param>
        /// <returns>The array of records.</returns>
        public IEnumerable<FileCabinetRecord> FindByFavouriteGame(string favGame)
        {
            return FindInCache(this.favGameDictionary, this.favGameCache, favGame);
        }

        /// <summary>
        /// Searches the records by donations.
        /// </summary>
        /// <param name="donations">Given donations.</param>
        /// <returns>The array of records.</returns>
        public IEnumerable<FileCabinetRecord> FindByDonations(string donations)
        {
            return FindInCache(this.donationsDictionary, this.donationsCache, donations);
        }

        /// <summary>
        /// Gets all the records.
        /// </summary>
        /// <returns>The array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            ReadOnlyCollection<FileCabinetRecord> listCopied = new ReadOnlyCollection<FileCabinetRecord>(this.list);
            return listCopied;
        }

        /// <summary>
        /// Makes a snapshot of records in the concrete moment.
        /// </summary>
        /// <returns>An instance of the IFileCabinetServiceSnapshot class.</returns>
        public IFileCabinetServiceSnapshot MakeSnapshot()
        {
            ReadOnlyCollection<FileCabinetRecord> records = new ReadOnlyCollection<FileCabinetRecord>(this.list);
            return new FileCabinetServiceSnapshot(records);
        }

        /// <summary>
        /// Restores the list of records by the snapshot.
        /// </summary>
        /// <param name="snapshot">Snapshot to restore by.</param>
        /// <returns>Number of imported records.</returns>
        public int Restore(IFileCabinetServiceSnapshot snapshot)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException($"Snapshot is invalid.");
            }

            int recordsImported = 0;

            foreach (var record in snapshot.Records)
            {
                string exceptionMessage = this.validator.Validate(record);
                if (exceptionMessage == null)
                {
                    if (this.ids.Contains(record.Id))
                    {
                        this.EditRecord(record);
                    }
                    else
                    {
                        this.AddRecord(record);
                    }

                    recordsImported++;
                }
                else
                {
                    Console.WriteLine("#" + record.Id + " record is invalid.");
                }
            }

            return recordsImported;
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
        /// Returns the number of deleted records in the list.
        /// </summary>
        /// <returns>The number of deleted records.</returns>
        public int GetDeleted()
        {
            return Deleted;
        }

        /// <summary>
        /// Returns list of ids in the list.
        /// </summary>
        /// <returns>List of ids in the list.</returns>
        public List<int> GetIds()
        {
            return this.ids;
        }

        /// <summary>
        /// Gets the validator.
        /// </summary>
        /// <returns>Validator.</returns>
        public IRecordValidator GetValidator()
        {
            return this.validator;
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

        private static IEnumerable<FileCabinetRecord> FindInCache(Dictionary<string, List<FileCabinetRecord>> dictionary, Dictionary<string, IEnumerable<FileCabinetRecord>> cache, string key)
        {
            IEnumerable<FileCabinetRecord> foundRecords;

            if (cache.ContainsKey(key))
            {
                foundRecords = cache[key];
            }
            else
            {
                foundRecords = FindByKey(key, dictionary);
                cache.Add(key, foundRecords);
            }

            return foundRecords;
        }

        /// <summary>
        /// Searches for the records in the dictionary by given key.
        /// </summary>
        /// <param name="key">The key to search by.</param>
        /// <param name="dictionary">The dictionary to search in.</param>
        /// <returns>The array of records.</returns>
        private static IEnumerable<FileCabinetRecord> FindByKey(string key, Dictionary<string, List<FileCabinetRecord>> dictionary)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key to find by is invalid");
            }

            if (dictionary.ContainsKey(key.ToLower()))
            {
                IEnumerable<FileCabinetRecord> foundRecords = dictionary[key.ToLower()];
                return foundRecords;
            }
            else
            {
                return null;
            }
        }

        private static void PurgeCache(Dictionary<string, IEnumerable<FileCabinetRecord>> cache, string key)
        {
            if (cache.ContainsKey(key))
            {
                cache.Remove(key);
            }
        }

        /// <summary>
        /// Updates all the dictionaries.
        /// </summary>
        /// <param name="record">Record to get values from.</param>
        private void UpdateDictionaries(FileCabinetRecord record)
        {
            UpdateDictionary(record, this.firstNameDictionary, record.FirstName);

            UpdateDictionary(record, this.lastNameDictionary, record.LastName);

            UpdateDictionary(record, this.dateOfBirthDictionary, record.DateOfBirth.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture));

            UpdateDictionary(record, this.favNumberDictionary, record.FavouriteNumber.ToString(CultureInfo.InvariantCulture));

            UpdateDictionary(record, this.favCharacterDictionary, record.FavouriteCharacter.ToString(CultureInfo.InvariantCulture));

            UpdateDictionary(record, this.favGameDictionary, record.FavouriteGame);

            UpdateDictionary(record, this.donationsDictionary, record.Donations.ToString(CultureInfo.InvariantCulture));
        }

        private void PurgeCache(FileCabinetRecord record)
        {
            PurgeCache(this.firstNameCache, record.FirstName);
            PurgeCache(this.lastNameCache, record.LastName);
            PurgeCache(this.dateOfBirthCache, record.DateOfBirth.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture));
        }

        private void RemoveFromDictionary(int indexToRemove)
        {
            this.firstNameDictionary[this.list[indexToRemove].FirstName.ToLower()].Remove(this.list[indexToRemove]);
            this.lastNameDictionary[this.list[indexToRemove].LastName.ToLower()].Remove(this.list[indexToRemove]);
            this.dateOfBirthDictionary[this.list[indexToRemove].DateOfBirth.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture).ToLower()].Remove(this.list[indexToRemove]);
            this.favNumberDictionary[this.list[indexToRemove].FavouriteNumber.ToString(CultureInfo.InvariantCulture).ToLower()].Remove(this.list[indexToRemove]);
            this.favCharacterDictionary[this.list[indexToRemove].FavouriteCharacter.ToString(CultureInfo.InvariantCulture).ToLower()].Remove(this.list[indexToRemove]);
            this.favGameDictionary[this.list[indexToRemove].FavouriteGame.ToString(CultureInfo.InvariantCulture).ToLower()].Remove(this.list[indexToRemove]);
            this.donationsDictionary[this.list[indexToRemove].Donations.ToString(CultureInfo.InvariantCulture).ToLower()].Remove(this.list[indexToRemove]);
        }
    }
}