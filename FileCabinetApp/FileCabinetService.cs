namespace FileCabinetApp
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using FileCabinetApp.Interfaces;

    /// <summary>
    /// Class for working with list of users.
    /// </summary>
    public class FileCabinetService : IFileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        private readonly Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private readonly Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        private readonly IRecordValidator validator;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetService"/> class.
        /// </summary>
        /// <param name="validator">Special validator.</param>
        public FileCabinetService(IRecordValidator validator)
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

            string validationException = this.validator.ValidateParameters(record);
            if (validationException != null)
            {
                return -1;
            }

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
        /// <returns>Whether operation succeeded.</returns>
        public int EditRecord(FileCabinetRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException($"Record object is invalid.");
            }

            if (record.Id < 0 || record.Id > this.GetStat())
            {
                return -1;
            }

            string validationException = this.validator.ValidateParameters(record);
            if (validationException != null)
            {
                throw new ArgumentException(validationException);
            }

            this.firstNameDictionary[this.list[record.Id].FirstName.ToLower()].Remove(this.list[record.Id]);
            this.lastNameDictionary[this.list[record.Id].LastName.ToLower()].Remove(this.list[record.Id]);
            this.dateOfBirthDictionary[this.list[record.Id].DateOfBirth.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture).ToLower()].Remove(this.list[record.Id]);

            this.list[record.Id] = record;

            UpdateDictionary(this.list[record.Id], this.firstNameDictionary, record.FirstName);

            UpdateDictionary(this.list[record.Id], this.lastNameDictionary, record.LastName);

            UpdateDictionary(this.list[record.Id], this.dateOfBirthDictionary, record.DateOfBirth.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture));

            this.list[record.Id].Id++;

            return 0;
        }

        /// <summary>
        /// Searches the records by first name.
        /// </summary>
        /// <param name="firstName">Given first name.</param>
        /// <returns>The array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            ReadOnlyCollection<FileCabinetRecord> foundRecords = FindByKey(firstName, this.firstNameDictionary);
            return foundRecords;
        }

        /// <summary>
        /// Searches the records by last name.
        /// </summary>
        /// <param name="lastName">Given last name.</param>
        /// <returns>The array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            ReadOnlyCollection<FileCabinetRecord> foundRecords = FindByKey(lastName, this.lastNameDictionary);
            return foundRecords;
        }

        /// <summary>
        /// Searches the records by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">Given date of birth.</param>
        /// <returns>The array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            ReadOnlyCollection<FileCabinetRecord> foundRecords = FindByKey(dateOfBirth, this.dateOfBirthDictionary);
            return foundRecords;
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
        /// Returns the number of records in the list.
        /// </summary>
        /// <returns>The number of records.</returns>
        public int GetStat()
        {
            return this.list.Count;
        }

        /// <summary>
        /// Gets the validator type.
        /// </summary>
        /// <returns>The type of validator in string form.</returns>
        public string GetValidatorType()
        {
            int validatorIndex = this.validator.GetType().ToString().IndexOf("Validator", StringComparison.InvariantCulture);
            string validationType = this.validator.GetType().ToString()[15..validatorIndex].ToLower();
            return validationType;
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

        /// <summary>
        /// Searches for the records in the dictionary by given key.
        /// </summary>
        /// <param name="key">The key to search by.</param>
        /// <param name="dictionary">The dictionary to search in.</param>
        /// <returns>The array of records.</returns>
        private static ReadOnlyCollection<FileCabinetRecord> FindByKey(string key, Dictionary<string, List<FileCabinetRecord>> dictionary)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key to find by is invalid");
            }

            if (dictionary.ContainsKey(key.ToLower()))
            {
                ReadOnlyCollection<FileCabinetRecord> foundRecords = new ReadOnlyCollection<FileCabinetRecord>(dictionary[key.ToLower()]);
                return foundRecords;
            }
            else
            {
                throw new ArgumentException("No records found.");
            }
        }

        /// <summary>
        /// Snapshot of records and methods saving them.
        /// </summary>
        internal class FileCabinetServiceSnapshot : IFileCabinetServiceSnapshot
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
            /// </summary>
            /// <param name="records">Records to snapshot.</param>
            public FileCabinetServiceSnapshot(ReadOnlyCollection<FileCabinetRecord> records)
            {
                this.Records = records;
            }

            /// <summary>
            /// Gets a snapshot of records.
            /// </summary>
            /// <value>A snapshot of records in the concrete moment.</value>
            public ReadOnlyCollection<FileCabinetRecord> Records { get; }

            /// <summary>
            /// Saves the records to csv file.
            /// </summary>
            /// <param name="writer">Writer.</param>
            /// <returns>Whether operation succeeded.</returns>
            public bool SaveToCsv(StreamWriter writer)
            {
                FileCabinetRecordCsvWriter csvWriter = new FileCabinetRecordCsvWriter(writer);
                foreach (var record in this.Records)
                {
                    csvWriter.Write(record);
                }

                return true;
            }

            /// <summary>
            /// Saves information to csv file.
            /// </summary>
            internal class FileCabinetRecordCsvWriter : IFileCabinetRecordCsvWriter
            {
                private readonly TextWriter writer;

                /// <summary>
                /// Initializes a new instance of the <see cref="FileCabinetRecordCsvWriter"/> class.
                /// </summary>
                /// <param name="writer">Special writer.</param>
                public FileCabinetRecordCsvWriter(TextWriter writer)
                {
                    this.writer = writer;
                }

                /// <summary>
                /// Writes information to csv file.
                /// </summary>
                /// <param name="record">Record to write about.</param>
                /// <returns>Whether operation succeeded.</returns>
                public bool Write(FileCabinetRecord record)
                {
                    if (record == null)
                    {
                        return false;
                    }

                    PropertyInfo[] properties = record.GetType().GetProperties();

                    for (int i = 0; i < properties.Length; i++)
                    {
                        if (properties[i].PropertyType == typeof(DateTime))
                        {
                            DateTime date = (DateTime)properties[i].GetValue(record);
                            this.writer.Write(date.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            this.writer.Write(properties[i].GetValue(record));
                        }

                        if (i != properties.Length - 1)
                        {
                            this.writer.Write(',');
                        }
                    }

                    this.writer.Write("\n");
                    return true;
                }
            }
        }
    }
}