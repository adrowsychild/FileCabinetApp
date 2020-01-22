using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using FileCabinetApp.Helpers;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp
{
    /// <summary>
    /// Class for working with list of users.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        /// <summary>
        /// The size of the record in bytes.
        /// </summary>
        public const int RecordSize = 400;
        private const int StringSize = 120;

        private readonly FileStream fileStream;

        private readonly IRecordValidator validator;

        // field -> record's offset of field in the file
        private SortedList<int, int> recordIdOffset = new SortedList<int, int>();
        private SortedList<string, List<int>> recordFirstNameOffset = new SortedList<string, List<int>>();
        private SortedList<string, List<int>> recordLastNameOffset = new SortedList<string, List<int>>();
        private SortedList<DateTime, List<int>> recordDateOfBirthOffset = new SortedList<DateTime, List<int>>();
        private SortedList<short, List<int>> recordFavNumberOffset = new SortedList<short, List<int>>();
        private SortedList<char, List<int>> recordFavCharacterOffset = new SortedList<char, List<int>>();
        private SortedList<string, List<int>> recordFavGameOffset = new SortedList<string, List<int>>();
        private SortedList<decimal, List<int>> recordDonationsOffset = new SortedList<decimal, List<int>>();

        private int count;
        private int deleted;

        private List<int> ids = new List<int>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// </summary>
        /// <param name="fileStream">FileStream with opened binary file.</param>
        /// <param name="validator">Specific validator.</param>
        public FileCabinetFilesystemService(FileStream fileStream, IRecordValidator validator)
        {
            this.fileStream = fileStream;
            this.validator = validator;
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="FileCabinetFilesystemService"/> class.
        /// Closes the stream.
        /// </summary>
        ~FileCabinetFilesystemService()
        {
            this.fileStream.Close();
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

            string exceptionMessage = this.validator.Validate(record);
            if (exceptionMessage != null)
            {
                return -1;
            }

            if (this.ids.Count == 0)
            {
                record.Id = 1;
            }
            else
            {
                record.Id = this.ids.Max() + 1;
            }

            this.count++;
            this.ids.Add(record.Id);
            this.WriteRecord(record, (this.count - 1) * RecordSize);
            this.UpdateOffsets(record, (this.count - 1) * RecordSize);

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
                int recordOffset = this.recordIdOffset[record.Id];
                FileCabinetRecord prevRecord = this.ReadRecord(recordOffset);
                this.DeleteOffsets(prevRecord, recordOffset);
                this.WriteRecord(record, recordOffset);
                this.UpdateOffsets(record, recordOffset);
            }
            else
            {
                this.ids.Add(record.Id);
                this.count++;
                this.WriteRecord(record, (this.count - 1) * RecordSize);
                this.UpdateOffsets(record, (this.count - 1) * RecordSize);
            }

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

            int recordOffset = this.recordIdOffset[record.Id];
            FileCabinetRecord prevRecord = this.ReadRecord(recordOffset);
            this.WriteRecord(record, recordOffset);
            this.DeleteOffsets(prevRecord, recordOffset);
            this.UpdateOffsets(record, recordOffset);

            return record.Id;
        }

        /// <summary>
        /// Removes a record from the list by given id.
        /// </summary>
        /// <param name="id">Id to remove record by.</param>
        /// <returns>Id of removed record if succeeded, -1 otherwise.
        /// </returns>
        public int RemoveRecord(int id)
        {
            int offset = this.recordIdOffset[id];
            if (offset == -1)
            {
                return -1;
            }

            FileCabinetRecord prevRecord = this.ReadRecord(offset);
            this.DeleteOffsets(prevRecord, offset);

            // 0x0100
            ushort numToAdd = 4;
            byte[] reservedBytes = new byte[2];
            this.fileStream.Seek(offset, SeekOrigin.Begin);
            this.fileStream.Read(reservedBytes, 0, 2);
            reservedBytes = BitConverter.GetBytes((ushort)(BitConverter.ToUInt16(reservedBytes) | numToAdd));
            this.fileStream.Seek(offset, SeekOrigin.Begin);
            this.fileStream.Write(reservedBytes, 0, 2);
            this.ids.Remove(id);
            this.deleted++;

            return id;
        }

        /// <summary>
        /// Purges the list of records.
        /// </summary>
        /// <returns>The number of purged records,
        /// -1 if not all the records were purged.
        /// </returns>
        public int Purge()
        {
            this.PurgeIds();

            int recordsPurged = 0;
            List<int> indexes = new List<int>();

            for (int i = 0; i < this.count; i++)
            {
                if (this.IsDeleted(RecordSize * i))
                {
                    indexes.Add(i);
                }
                else
                {
                    continue;
                }
            }

            int currentIndex = 0;
            int numOfRecords = this.count;
            FileCabinetRecord currentRecord;

            for (int i = 0; i < numOfRecords; i++)
            {
                if (!indexes.Contains(i))
                {
                    if (i == currentIndex)
                    {
                        currentIndex++;
                        continue;
                    }

                    currentRecord = this.ReadRecord(i * RecordSize);
                    this.WriteRecord(currentRecord, currentIndex * RecordSize);
                    currentIndex++;
                }
                else
                {
                    recordsPurged++;
                    continue;
                }
            }

            this.count -= recordsPurged;

            if (this.deleted == recordsPurged)
            {
                this.deleted = 0;
                return recordsPurged;
            }
            else
            {
                return -1;
            }
        }

        /// <summary>
        /// Searches the records by id.
        /// </summary>
        /// <param name="id">Given id.</param>
        /// <returns>The array of records.</returns>
        public IEnumerable<FileCabinetRecord> FindById(string id)
        {
            if (id == null || !int.TryParse(id, out int parsedId))
            {
                return null;
            }

            return new FileSystemRecordCollection(this, new ReadOnlyCollection<FileCabinetRecord>(new List<FileCabinetRecord>() { this.GetRecord(parsedId) }));
        }

        /// <summary>
        /// Searches the records by first name.
        /// </summary>
        /// <param name="firstName">Given first name.</param>
        /// <returns>The array of records.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            if (firstName == null)
            {
                return null;
            }

            return this.FindByKey(this.recordFirstNameOffset, firstName.ToLower());
        }

        /// <summary>
        /// Searches the records by last name.
        /// </summary>
        /// <param name="lastName">Given last name.</param>
        /// <returns>The array of records.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            if (lastName == null)
            {
                return null;
            }

            return this.FindByKey(this.recordLastNameOffset, lastName.ToLower());
        }

        /// <summary>
        /// Searches the records by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">Given date of birth.</param>
        /// <returns>The array of records.</returns>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            if (dateOfBirth == null)
            {
                return null;
            }

            if (!DateTime.TryParseExact(dateOfBirth, "yyyy-MMM-d", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDateOfBirth))
            {
                return null;
            }

            return this.FindByKey(this.recordDateOfBirthOffset, parsedDateOfBirth);
        }

        /// <summary>
        /// Searches the records by favourite number.
        /// </summary>
        /// <param name="favNumber">Given favourite number.</param>
        /// <returns>The array of records.</returns>
        public IEnumerable<FileCabinetRecord> FindByFavouriteNumber(string favNumber)
        {
            if (favNumber == null)
            {
                return null;
            }

            if (!short.TryParse(favNumber, out short parsedFavNumver))
            {
                return null;
            }

            return this.FindByKey(this.recordFavNumberOffset, parsedFavNumver);
        }

        /// <summary>
        /// Searches the records by favourite character.
        /// </summary>
        /// <param name="favCharacter">Given favourite character.</param>
        /// <returns>The array of records.</returns>
        public IEnumerable<FileCabinetRecord> FindByFavouriteCharacter(string favCharacter)
        {
            if (favCharacter == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(favCharacter) || favCharacter.Length > 1)
            {
                return null;
            }

            return this.FindByKey(this.recordFavCharacterOffset, favCharacter);
        }

        /// <summary>
        /// Searches the records by favourite game.
        /// </summary>
        /// <param name="favGame">Given favourite game.</param>
        /// <returns>The array of records.</returns>
        public IEnumerable<FileCabinetRecord> FindByFavouriteGame(string favGame)
        {
            if (favGame == null)
            {
                return null;
            }

            return this.FindByKey(this.recordFavGameOffset, favGame.ToLower());
        }

        /// <summary>
        /// Searches the records by donations.
        /// </summary>
        /// <param name="donations">Given donations.</param>
        /// <returns>The array of records.</returns>
        public IEnumerable<FileCabinetRecord> FindByDonations(string donations)
        {
            if (donations == null)
            {
                return null;
            }

            if (!decimal.TryParse(donations, out decimal parsedDonations))
            {
                return null;
            }

            return this.FindByKey(this.recordDonationsOffset, parsedDonations);
        }

        /// <summary>
        /// Gets all the records.
        /// </summary>
        /// <returns>The array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            List<FileCabinetRecord> storedRecords = new List<FileCabinetRecord>();

            for (int i = 0; i < this.count; i++)
            {
                if (this.IsDeleted(RecordSize * i))
                {
                    continue;
                }

                storedRecords.Add(this.ReadRecord(i * RecordSize));
            }

            ReadOnlyCollection<FileCabinetRecord> records = new ReadOnlyCollection<FileCabinetRecord>(storedRecords);

            return records;
        }

        /// <summary>
        /// Returns the number of records in the list.
        /// </summary>
        /// <returns>The number of records.</returns>
        public int GetStat()
        {
            return this.count;
        }

        /// <summary>
        /// Returns the number of deleted records in the list.
        /// </summary>
        /// <returns>The number of deleted records.</returns>
        public int GetDeleted()
        {
            return this.deleted;
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
        /// Makes a snapshot of records in the concrete moment.
        /// </summary>
        /// <returns>An instance of the IFileCabinetServiceSnapshot class.</returns>
        public IFileCabinetServiceSnapshot MakeSnapshot()
        {
            ReadOnlyCollection<FileCabinetRecord> records = this.GetRecords();

            return new FileCabinetServiceSnapshot(records);
        }

        /// <summary>
        /// Restores the records from the snapshot.
        /// </summary>
        /// <param name="snapshot">A snapshot to restore.</param>
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
                        recordsImported++;
                    }
                    else
                    {
                        this.AddRecord(record);
                        recordsImported++;
                    }
                }
                else
                {
                    Console.WriteLine("#" + record.Id + " record is invalid.");
                }
            }

            return recordsImported;
        }

        /// <summary>
        /// Reads the record from the file.
        /// </summary>
        /// <param name="offset">Offset to read from.</param>
        /// <returns>Record read.</returns>
        public FileCabinetRecord ReadRecord(int offset)
        {
            this.fileStream.Seek(offset, SeekOrigin.Begin);
            byte[] reservedBytes = new byte[2];
            this.fileStream.Read(reservedBytes, 0, 2);
            byte[] bytes = new byte[sizeof(int)];
            this.fileStream.Read(bytes, 0, sizeof(int));
            int id = BitConverter.ToInt32(bytes);
            bytes = new byte[StringSize];
            this.fileStream.Read(bytes, 0, StringSize);
            string firstName = WorkWithBytesHelper.RemoveOffset(Encoding.ASCII.GetString(bytes));
            this.fileStream.Read(bytes, 0, StringSize);
            string lastName = WorkWithBytesHelper.RemoveOffset(Encoding.ASCII.GetString(bytes));
            bytes = new byte[sizeof(int)];
            this.fileStream.Read(bytes, 0, sizeof(int));
            int year = BitConverter.ToInt32(bytes);
            this.fileStream.Read(bytes, 0, sizeof(int));
            int month = BitConverter.ToInt32(bytes);
            this.fileStream.Read(bytes, 0, sizeof(int));
            int day = BitConverter.ToInt32(bytes);
            bytes = new byte[sizeof(short)];
            this.fileStream.Read(bytes, 0, sizeof(short));
            short favNumber = BitConverter.ToInt16(bytes);
            bytes = new byte[sizeof(char)];
            this.fileStream.Read(bytes, 0, sizeof(char));
            char favCharacter = BitConverter.ToChar(bytes);
            bytes = new byte[StringSize];
            this.fileStream.Read(bytes, 0, StringSize);
            string favGame = WorkWithBytesHelper.RemoveOffset(Encoding.ASCII.GetString(bytes));
            bytes = new byte[sizeof(decimal)];
            this.fileStream.Read(bytes, 0, sizeof(decimal));
            decimal donations = WorkWithBytesHelper.BytesToDecimal(bytes);

            return new FileCabinetRecord(id, firstName, lastName, new DateTime(year, month, day), favNumber, favCharacter, favGame, donations);
        }

        /// <summary>
        /// Writes a record to the file.
        /// </summary>
        /// <param name="record">A record to write.</param>
        /// <param name="offset">Offset to start from.</param>
        private void WriteRecord(FileCabinetRecord record, int offset)
        {
            this.fileStream.Seek(offset, SeekOrigin.Begin);
            this.fileStream.Write(new byte[2], 0, sizeof(short));
            this.fileStream.Write(BitConverter.GetBytes(record.Id), 0, sizeof(int));
            this.fileStream.Write(WorkWithBytesHelper.MakeStringOffset(record.FirstName, StringSize), 0, StringSize);
            this.fileStream.Write(WorkWithBytesHelper.MakeStringOffset(record.LastName, StringSize), 0, StringSize);
            this.fileStream.Write(BitConverter.GetBytes(record.DateOfBirth.Year), 0, sizeof(int));
            this.fileStream.Write(BitConverter.GetBytes(record.DateOfBirth.Month), 0, sizeof(int));
            this.fileStream.Write(BitConverter.GetBytes(record.DateOfBirth.Day), 0, sizeof(int));
            this.fileStream.Write(BitConverter.GetBytes(record.FavouriteNumber), 0, sizeof(short));
            this.fileStream.Write(BitConverter.GetBytes(record.FavouriteCharacter), 0, sizeof(char));
            this.fileStream.Write(WorkWithBytesHelper.MakeStringOffset(record.FavouriteGame, StringSize), 0, StringSize);
            this.fileStream.Write(WorkWithBytesHelper.DecimalToBytes(record.Donations), 0, sizeof(decimal));
        }

        /// <summary>
        /// Removes duplicates from the ids list.
        /// </summary>
        private void PurgeIds()
        {
            this.ids = this.ids.Distinct().ToList();
        }

        private bool IsDeleted(int offset)
        {
            // 0x0100
            byte[] reservedBytes = new byte[2];
            this.fileStream.Seek(offset, SeekOrigin.Begin);
            this.fileStream.Read(reservedBytes, 0, 2);

            if ((ushort)((BitConverter.ToUInt16(reservedBytes) >> 2) & 1) == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Updates a single dictionary containing offset.
        /// </summary>
        /// <typeparam name="T">Specifies the type of property.</typeparam>
        /// <param name="dictionary">Dictionary to update.</param>
        /// <param name="value">Key to add or find by.</param>
        /// <param name="offset">Offset to add.</param>
        private void UpdateDictionary<T>(SortedList<T, List<int>> dictionary, T value, int offset)
        {
            if (!dictionary.ContainsKey(value))
            {
                List<int> listOfOffsets = new List<int>
                {
                    offset,
                };

                dictionary.Add(value, listOfOffsets);
            }
            else
            {
                dictionary[value].Add(offset);
            }
        }

        /// <summary>
        /// Updates all the dictionaries containing offsets.
        /// </summary>
        /// <param name="record">Record to add properties from.</param>
        /// <param name="recordOffset">Record's offset in the file.</param>
        private void UpdateOffsets(FileCabinetRecord record, int recordOffset)
        {
            this.recordIdOffset.Add(record.Id, recordOffset);
            this.UpdateDictionary(this.recordFirstNameOffset, record.FirstName, recordOffset);
            this.UpdateDictionary(this.recordLastNameOffset, record.LastName, recordOffset);
            this.UpdateDictionary(this.recordDateOfBirthOffset, record.DateOfBirth, recordOffset);
            this.UpdateDictionary(this.recordFavNumberOffset, record.FavouriteNumber, recordOffset);
            this.UpdateDictionary(this.recordFavCharacterOffset, record.FavouriteCharacter, recordOffset);
            this.UpdateDictionary(this.recordFavGameOffset, record.FavouriteGame, recordOffset);
            this.UpdateDictionary(this.recordDonationsOffset, record.Donations, recordOffset);
        }

        /// <summary>
        /// Deletes from all the dictionaries containing offsets.
        /// </summary>
        /// <param name="record">Record to delelte properties from.</param>
        /// <param name="recordOffset">Record's offset in the file.</param>
        private void DeleteOffsets(FileCabinetRecord record, int recordOffset)
        {
            this.recordIdOffset.Remove(record.Id);
            this.recordFirstNameOffset[record.FirstName.ToLower()].Remove(recordOffset);
            this.recordLastNameOffset[record.LastName.ToLower()].Remove(recordOffset);
            this.recordDateOfBirthOffset[record.DateOfBirth].Remove(recordOffset);
            this.recordFavNumberOffset[record.FavouriteNumber].Remove(recordOffset);
            this.recordFavCharacterOffset[record.FavouriteCharacter].Remove(recordOffset);
            this.recordFavGameOffset[record.FavouriteGame.ToLower()].Remove(recordOffset);
            this.recordDonationsOffset[record.Donations].Remove(recordOffset);
        }

        private FileSystemRecordCollection FindByKey<T>(SortedList<T, List<int>> offsetDictionary, dynamic key)
        {
            List<FileCabinetRecord> storedRecords = new List<FileCabinetRecord>();

            if (offsetDictionary.ContainsKey(key))
            {
                List<int> offsets = offsetDictionary[key];

                foreach (int offset in offsets)
                {
                    if (this.IsDeleted(offset))
                    {
                        continue;
                    }

                    storedRecords.Add(this.ReadRecord(offset));
                }
            }

            if (storedRecords.Count == 0)
            {
                return null;
            }

            return new FileSystemRecordCollection(this, new ReadOnlyCollection<FileCabinetRecord>(storedRecords));
        }

        /// <summary>
        /// Gets a single method from the list of records.
        /// </summary>
        /// <param name="id">Id to get record by.</param>
        /// <returns>The record found, null otherwise.</returns>
        private FileCabinetRecord GetRecord(int id)
        {
            if (this.IsDeleted(this.recordIdOffset[id]))
            {
                return null;
            }
            else
            {
                return this.ReadRecord(this.recordIdOffset[id]);
            }
        }
    }
}
