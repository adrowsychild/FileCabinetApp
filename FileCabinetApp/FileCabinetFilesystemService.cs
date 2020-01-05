﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using FileCabinetApp.Interfaces;
using static System.Decimal;
using static System.String;

namespace FileCabinetApp
{
    /// <summary>
    /// Class for working with list of users.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int RecordSize = 400;

        private readonly FileStream fileStream;

        private readonly IRecordValidator validator;

        private int count;

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

            record.Id = ++this.count;

            this.WriteRecord(record, this.count - 1);

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

            if (record.Id < 0 || record.Id > this.count)
            {
                return -1;
            }

            record.Id++;

            this.WriteRecord(record, record.Id - 1);

            return record.Id;
        }

        /// <summary>
        /// Searches the records by first name.
        /// </summary>
        /// <param name="firstName">Given first name.</param>
        /// <returns>The array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches the records by last name.
        /// </summary>
        /// <param name="lastName">Given last name.</param>
        /// <returns>The array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Searches the records by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">Given date of birth.</param>
        /// <returns>The array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            throw new NotImplementedException();
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
                storedRecords.Add(this.ReadRecord(i));
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
        /// Gets the validator.
        /// </summary>
        /// <returns>Validator.</returns>
        public IRecordValidator GetValidator()
        {
            return this.validator;
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
        /// Makes a snapshot of records in the concrete moment.
        /// </summary>
        /// <returns>An instance of the IFileCabinetServiceSnapshot class.</returns>
        public IFileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Closes the stream.
        /// </summary>
        public void Close()
        {
            this.fileStream.Close();
        }

        private static byte[] MakeStringOffset(string value)
        {
            byte[] bytes = new byte[120];
            byte[] somebytes = Encoding.ASCII.GetBytes(value);
            for (int i = 0; i < somebytes.Length; i++)
            {
                bytes[i] = somebytes[i];
            }

            return bytes;
        }

        private static string RemoveOffset(string value)
        {
            int i = 0;
            int actualLength = 1;

            while (i < value.Length && value[i] != 0x0)
            {
                actualLength++;
                i++;
            }

            value = value.Substring(0, i);

            return value;
        }

        private static byte[] DecimalToBytes(decimal value)
        {
            byte[] bytes = new byte[16];

            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(value);
                    bytes = stream.ToArray();
                }
            }

            return bytes;
        }

        private static decimal BytesToDecimal(byte[] bytes)
        {
            decimal value;

            using (MemoryStream stream = new MemoryStream(bytes))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    value = reader.ReadDecimal();
                }
            }

            return value;
        }

        /// <summary>
        /// Writes a record to the file.
        /// </summary>
        /// <param name="record">A record to write.</param>
        private void WriteRecord(FileCabinetRecord record, int index)
        {
            this.fileStream.Seek(index * RecordSize, SeekOrigin.Begin);
            this.fileStream.Write(new byte[2], 0, sizeof(short));
            this.fileStream.Write(BitConverter.GetBytes(record.Id), 0, sizeof(int));
            this.fileStream.Write(MakeStringOffset(record.FirstName), 0, 120);
            this.fileStream.Write(MakeStringOffset(record.LastName), 0, 120);
            this.fileStream.Write(BitConverter.GetBytes(record.DateOfBirth.Year), 0, sizeof(int));
            this.fileStream.Write(BitConverter.GetBytes(record.DateOfBirth.Month), 0, sizeof(int));
            this.fileStream.Write(BitConverter.GetBytes(record.DateOfBirth.Day), 0, sizeof(int));
            this.fileStream.Write(BitConverter.GetBytes(record.FavouriteNumber), 0, sizeof(short));
            this.fileStream.Write(BitConverter.GetBytes(record.FavouriteCharacter), 0, sizeof(char));
            this.fileStream.Write(MakeStringOffset(record.FavouriteGame), 0, 120);
            this.fileStream.Write(DecimalToBytes(record.Donations), 0, sizeof(decimal));
        }

        /// <summary>
        /// Reads the record from the file.
        /// </summary>
        /// <param name="index">Index to read by.</param>
        private FileCabinetRecord ReadRecord(int index)
        {
            this.fileStream.Seek((index * RecordSize) + 2, SeekOrigin.Begin);
            byte[] bytes = new byte[4];
            this.fileStream.Read(bytes, 0, sizeof(int));
            int tmpId = BitConverter.ToInt32(bytes);
            bytes = new byte[120];
            this.fileStream.Read(bytes, 0, 120);
            string tmpFirstName = Encoding.ASCII.GetString(bytes);
            tmpFirstName = RemoveOffset(tmpFirstName);
            this.fileStream.Read(bytes, 0, 120);
            string tmpLastName = Encoding.ASCII.GetString(bytes);
            tmpLastName = RemoveOffset(tmpFirstName);
            bytes = new byte[4];
            this.fileStream.Read(bytes, 0, sizeof(int));
            int tmpYear = BitConverter.ToInt32(bytes);
            this.fileStream.Read(bytes, 0, sizeof(int));
            int tmpMonth = BitConverter.ToInt32(bytes);
            this.fileStream.Read(bytes, 0, sizeof(int));
            int tmpDay = BitConverter.ToInt32(bytes);
            bytes = new byte[2];
            this.fileStream.Read(bytes, 0, sizeof(short));
            short tmpFavNumber = BitConverter.ToInt16(bytes);
            this.fileStream.Read(bytes, 0, sizeof(char));
            char tmpFavCharacter = BitConverter.ToChar(bytes);
            bytes = new byte[120];
            this.fileStream.Read(bytes, 0, 120);
            string tmpFavGame = Encoding.ASCII.GetString(bytes);
            tmpFavGame = RemoveOffset(tmpFavGame);
            bytes = new byte[16];
            this.fileStream.Read(bytes, 0, sizeof(decimal));
            decimal tmpDonations = BytesToDecimal(bytes);

            return new FileCabinetRecord(tmpId, tmpFirstName, tmpLastName, new DateTime(tmpYear, tmpMonth, tmpDay), tmpFavNumber, tmpFavCharacter, tmpFavGame, tmpDonations);
        }
    }
}
