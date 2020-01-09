using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp
{
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
}
