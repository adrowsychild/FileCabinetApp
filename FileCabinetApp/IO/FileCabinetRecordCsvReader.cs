using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp
{
    /// <summary>
    /// Loads information from csv file.
    /// </summary>
    public class FileCabinetRecordCsvReader : IFileCabinetRecordCsvReader
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
}
