using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp
{
    /// <summary>
    /// Class for working with list of users.
    /// </summary>
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        private const int RecordSize = 400;
        private const int StringSize = 120;

        private const int IdOffset = 4;
        private const int FirstNameOffset = 6;
        private const int LastNameOffset = 126;
        private const int YearOffset = 246;
        private const int MonthOffset = 250;
        private const int DayOffset = 254;
        private const int FavNumOffset = 258;
        private const int FavCarOffset = 262;
        private const int FavGameOffset = 382;
        private const int DonationsOffset = 398;

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
            List<FileCabinetRecord> storedRecords = new List<FileCabinetRecord>();

            for (int i = 0; i < this.count; i++)
            {
                this.fileStream.Seek(FirstNameOffset + (RecordSize * i), SeekOrigin.Begin);
                byte[] bytes = new byte[StringSize];
                this.fileStream.Read(bytes, 0, StringSize);
                string tmpFirstName = Encoding.ASCII.GetString(bytes);
                tmpFirstName = RemoveOffset(tmpFirstName);
                if (tmpFirstName.ToLower() == firstName.ToLower())
                {
                    storedRecords.Add(this.ReadRecord(i));
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(storedRecords);
        }

        /// <summary>
        /// Searches the records by last name.
        /// </summary>
        /// <param name="lastName">Given last name.</param>
        /// <returns>The array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            List<FileCabinetRecord> storedRecords = new List<FileCabinetRecord>();

            for (int i = 0; i < this.count; i++)
            {
                this.fileStream.Seek(FirstNameOffset + (RecordSize * i), SeekOrigin.Begin);
                byte[] bytes = new byte[StringSize];
                this.fileStream.Read(bytes, 0, StringSize);
                string tmpLastName = Encoding.ASCII.GetString(bytes);
                tmpLastName = RemoveOffset(tmpLastName);
                if (tmpLastName.ToLower() == lastName.ToLower())
                {
                    storedRecords.Add(this.ReadRecord(i));
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(storedRecords);
        }

        /// <summary>
        /// Searches the records by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">Given date of birth.</param>
        /// <returns>The array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            List<FileCabinetRecord> storedRecords = new List<FileCabinetRecord>();

            for (int i = 0; i < this.count; i++)
            {
                this.fileStream.Seek(YearOffset + (RecordSize * i), SeekOrigin.Begin);
                byte[] bytes = new byte[4];
                this.fileStream.Read(bytes, 0, sizeof(int));
                int tmpYear = BitConverter.ToInt32(bytes);
                this.fileStream.Read(bytes, 0, sizeof(int));
                int tmpMonth = BitConverter.ToInt32(bytes);
                this.fileStream.Read(bytes, 0, sizeof(int));
                int tmpDay = BitConverter.ToInt32(bytes);
                DateTime tmpDateOfBirth = new DateTime(tmpYear, tmpMonth, tmpDay);
                if (tmpDateOfBirth.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture).ToLower() == dateOfBirth.ToLower())
                {
                    storedRecords.Add(this.ReadRecord(i));
                }
            }

            return new ReadOnlyCollection<FileCabinetRecord>(storedRecords);
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

        /// <summary>
        /// Makes an offset for the string.
        /// </summary>
        /// <param name="value">String to make offset to.</param>
        /// <returns>String with offset in bytes.</returns>
        private static byte[] MakeStringOffset(string value, int offset)
        {
            byte[] bytes = new byte[offset];
            byte[] somebytes = Encoding.ASCII.GetBytes(value);
            for (int i = 0; i < somebytes.Length; i++)
            {
                bytes[i] = somebytes[i];
            }

            return bytes;
        }

        /// <summary>
        /// Removes the offset from the string.
        /// </summary>
        /// <param name="value">String to remove offset from.</param>
        /// <returns>String without offset.</returns>
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

        /// <summary>
        /// Converts decimal value to array of bytes.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Array of bytes.</returns>
        private static byte[] DecimalToBytes(decimal value)
        {
            byte[] bytes = new byte[sizeof(decimal)];

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

        /// <summary>
        /// Converts array of bytes to decimal value.
        /// </summary>
        /// <param name="bytes">Bytes to convert.</param>
        /// <returns>Decimal value.</returns>
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
            this.fileStream.Write(MakeStringOffset(record.FirstName, StringSize), 0, StringSize);
            this.fileStream.Write(MakeStringOffset(record.LastName, StringSize), 0, StringSize);
            this.fileStream.Write(BitConverter.GetBytes(record.DateOfBirth.Year), 0, sizeof(int));
            this.fileStream.Write(BitConverter.GetBytes(record.DateOfBirth.Month), 0, sizeof(int));
            this.fileStream.Write(BitConverter.GetBytes(record.DateOfBirth.Day), 0, sizeof(int));
            this.fileStream.Write(BitConverter.GetBytes(record.FavouriteNumber), 0, sizeof(short));
            this.fileStream.Write(BitConverter.GetBytes(record.FavouriteCharacter), 0, sizeof(char));
            this.fileStream.Write(MakeStringOffset(record.FavouriteGame, StringSize), 0, StringSize);
            this.fileStream.Write(DecimalToBytes(record.Donations), 0, sizeof(decimal));
        }

        /// <summary>
        /// Reads the record from the file.
        /// </summary>
        /// <param name="index">Index to read by.</param>
        private FileCabinetRecord ReadRecord(int index)
        {
            this.fileStream.Seek((index * RecordSize) + 2, SeekOrigin.Begin);
            byte[] bytes = new byte[sizeof(int)];
            this.fileStream.Read(bytes, 0, sizeof(int));
            int id = BitConverter.ToInt32(bytes);
            bytes = new byte[StringSize];
            this.fileStream.Read(bytes, 0, StringSize);
            string firstName = RemoveOffset(Encoding.ASCII.GetString(bytes));
            this.fileStream.Read(bytes, 0, StringSize);
            string lastName = RemoveOffset(Encoding.ASCII.GetString(bytes));
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
            string favGame = RemoveOffset(Encoding.ASCII.GetString(bytes));
            bytes = new byte[sizeof(decimal)];
            this.fileStream.Read(bytes, 0, sizeof(decimal));
            decimal donations = BytesToDecimal(bytes);

            return new FileCabinetRecord(id, firstName, lastName, new DateTime(year, month, day), favNumber, favCharacter, favGame, donations);
        }
    }
}
