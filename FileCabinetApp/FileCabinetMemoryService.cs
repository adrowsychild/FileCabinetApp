namespace FileCabinetApp
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using FileCabinetApp.Interfaces;

    /// <summary>
    /// Class for working with list of users.
    /// </summary>
    public class FileCabinetMemoryService : IFileCabinetService
    {
        private readonly IRecordValidator validator;

        private List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        private Dictionary<string, List<FileCabinetRecord>> firstNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private Dictionary<string, List<FileCabinetRecord>> lastNameDictionary = new Dictionary<string, List<FileCabinetRecord>>();
        private Dictionary<string, List<FileCabinetRecord>> dateOfBirthDictionary = new Dictionary<string, List<FileCabinetRecord>>();

        private List<int> ids = new List<int>();

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

            record.Id = this.list.Count + 1;

            this.list.Add(record);

            UpdateDictionary(record, this.firstNameDictionary, record.FirstName);

            UpdateDictionary(record, this.lastNameDictionary, record.LastName);

            UpdateDictionary(record, this.dateOfBirthDictionary, record.DateOfBirth.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture));

            this.ids.Add(record.Id);

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

            int insideId = record.Id - 1;

            this.firstNameDictionary[this.list[insideId].FirstName.ToLower()].Remove(this.list[insideId]);
            this.lastNameDictionary[this.list[insideId].LastName.ToLower()].Remove(this.list[insideId]);
            this.dateOfBirthDictionary[this.list[insideId].DateOfBirth.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture).ToLower()].Remove(this.list[insideId]);
            this.ids.Remove(this.list[insideId].Id);

            this.list[insideId] = record;

            UpdateDictionary(this.list[insideId], this.firstNameDictionary, record.FirstName);

            UpdateDictionary(this.list[insideId], this.lastNameDictionary, record.LastName);

            UpdateDictionary(this.list[insideId], this.dateOfBirthDictionary, record.DateOfBirth.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture));

            this.ids.Add(record.Id);

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
        /// Makes a snapshot of records.
        /// </summary>
        /// <param name="recordsToSnapshot">Records to snapshot.</param>
        /// <returns>Snapshot.</returns>
        public IFileCabinetServiceSnapshot MakeSnapshot(List<FileCabinetRecord> recordsToSnapshot)
        {
            ReadOnlyCollection<FileCabinetRecord> records = new ReadOnlyCollection<FileCabinetRecord>(recordsToSnapshot);
            return new FileCabinetServiceSnapshot(records);
        }

        /// <summary>
        /// Makes an empty snapshot.
        /// </summary>
        /// <returns>Empty snapshot.</returns>
        public IFileCabinetServiceSnapshot MakeEmptySnapshot()
        {
            return new FileCabinetServiceSnapshot();
        }

        /// <summary>
        /// Restores the list of records by the snapshot.
        /// </summary>
        /// <param name="snapshot">Snapshot to restore by.</param>
        public void Restore(IFileCabinetServiceSnapshot snapshot)
        {
            if (snapshot == null)
            {
                throw new ArgumentNullException($"Snapshot is invalid.");
            }

            int recordsImported = 0;

            foreach (var record in snapshot.Records)
            {
                string exceptionMessage = this.validator.ValidateParameters(record);
                if (exceptionMessage == null)
                {
                    if (this.ids.Contains(record.Id))
                    {
                        this.EditRecord(record);
                        recordsImported++;
                    }
                }
                else
                {
                    Console.WriteLine("#" + record.Id + " record is invalid: " + exceptionMessage);
                }
            }
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
        /// Clears the list.
        /// </summary>
        public void Close()
        {
            this.list.Clear();
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
            public FileCabinetServiceSnapshot()
            {
            }

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
            /// <param name="writer">Csv writer.</param>
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
            /// Saves the records to xml file.
            /// </summary>
            /// <param name="writer">Xml writer.</param>
            /// <returns>Whether operation succeeded.</returns>
            public bool SaveToXml(StreamWriter writer)
            {
                XmlWriter xmlWriter = XmlWriter.Create(writer);
                FileCabinetXmlWriter fileXmlWriter = new FileCabinetXmlWriter(xmlWriter);

                xmlWriter.WriteStartDocument();
                xmlWriter.WriteStartElement("records");

                foreach (var record in this.Records)
                {
                    fileXmlWriter.Write(record);
                }

                xmlWriter.WriteEndDocument();
                xmlWriter.Close();

                return true;
            }

            /// <summary>
            /// Loads the records from csv file.
            /// </summary>
            /// <param name="reader">Csv reader.</param>
            /// <returns>Whether operation succeeded.</returns>
            public IList<FileCabinetRecord> LoadFromCsv(StreamReader reader)
            {
                FileCabinetRecordCsvReader csvReader = new FileCabinetRecordCsvReader(reader);
                IList<FileCabinetRecord> records = csvReader.ReadAll();

                return records;
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
                /// <param name="writer">Csv writer.</param>
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

            /// <summary>
            /// Loads information from csv file.
            /// </summary>
            internal class FileCabinetRecordCsvReader : IFileCabinetRecordCsvReader
            {
                private readonly TextReader reader;

                /// <summary>
                /// Initializes a new instance of the <see cref="FileCabinetRecordCsvReader"/> class.
                /// </summary>
                /// <param name="reader">Csv reader.</param>
                public FileCabinetRecordCsvReader(TextReader reader)
                {
                    this.reader = reader;
                }

                /// <summary>
                /// Reads records from csv file.
                /// </summary>
                /// <returns>List of records.</returns>
                public IList<FileCabinetRecord> ReadAll()
                {
                    IList<FileCabinetRecord> records = new List<FileCabinetRecord>();

                    while (this.reader.Peek() > -1)
                    {
                        records.Add(this.Read());
                    }

                    return records;
                }

                /// <summary>
                /// Reades a single record from csv file.
                /// </summary>
                /// <returns>Record.</returns>
                public FileCabinetRecord Read()
                {
                    string record = this.reader.ReadLine();
                    string[] values = record.Split(',');
                    int id = int.Parse(values[0], CultureInfo.InvariantCulture);
                    string firstName = values[1];
                    string lastName = values[2];
                    DateTime dateOfBirth = DateTime.ParseExact(values[3], "yyyy-MMM-d", CultureInfo.InvariantCulture);
                    short favNumber = (short)int.Parse(values[4], CultureInfo.InvariantCulture);
                    char favCharacter = values[5][0];
                    string favGame = values[6];
                    decimal donations = decimal.Parse(values[7], CultureInfo.InvariantCulture);

                    return new FileCabinetRecord(id, firstName, lastName, dateOfBirth, favNumber, favCharacter, favGame, donations);
                }
            }

            /// <summary>
            /// Saves information to xml file.
            /// </summary>
            internal class FileCabinetXmlWriter : IFileCabinetXmlWriter
            {
                private readonly XmlWriter writer;

                /// <summary>
                /// Initializes a new instance of the <see cref="FileCabinetXmlWriter"/> class.
                /// </summary>
                /// <param name="writer">Xml writer.</param>
                public FileCabinetXmlWriter(XmlWriter writer)
                {
                    this.writer = writer;
                }

                /// <summary>
                /// Writes information to xml file.
                /// </summary>
                /// <param name="record">Record to write about.</param>
                /// <returns>Whether operation succeeded.</returns>
                public bool Write(FileCabinetRecord record)
                {
                    if (record == null)
                    {
                        return false;
                    }

                    this.writer.WriteStartElement("record");
                    this.writer.WriteAttributeString("id", record.Id.ToString(CultureInfo.InvariantCulture));
                    this.writer.WriteStartElement("name");
                    this.writer.WriteAttributeString("first", record.FirstName);
                    this.writer.WriteAttributeString("last", record.LastName);
                    this.writer.WriteEndElement();
                    this.writer.WriteStartElement("dateOfBirth");
                    this.writer.WriteString(record.DateOfBirth.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture));
                    this.writer.WriteEndElement();
                    this.writer.WriteStartElement("favourite");
                    this.writer.WriteAttributeString("number", record.FavouriteNumber.ToString(CultureInfo.InvariantCulture));
                    this.writer.WriteAttributeString("character", record.FavouriteCharacter.ToString(CultureInfo.InvariantCulture));
                    this.writer.WriteAttributeString("game", record.FavouriteGame);
                    this.writer.WriteEndElement();
                    this.writer.WriteStartElement("donations");
                    this.writer.WriteString(record.Donations.ToString(CultureInfo.InvariantCulture));
                    this.writer.WriteEndElement();
                    this.writer.WriteEndElement();

                    return true;
                }
            }
        }
    }
}