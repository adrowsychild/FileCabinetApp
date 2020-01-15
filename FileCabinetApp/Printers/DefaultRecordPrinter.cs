using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.Printers
{
    /// <summary>
    /// Default printer for the records.
    /// </summary>
    public class DefaultRecordPrinter : IRecordPrinter
    {
        /// <summary>
        /// Prints the list of records to the user.
        /// </summary>
        /// <param name="records">Records to print.</param>
        public void Print(IEnumerable<FileCabinetRecord> records)
        {
            IEnumerable<FileCabinetRecord> orderedRecords = records.OrderBy(record => record.Id);
            foreach (var record in orderedRecords)
            {
                this.PrintRecord(record);
            }
        }

        /// <summary>
        /// Prints single record.
        /// </summary>
        /// <param name="record">Record to print.</param>
        private void PrintRecord(FileCabinetRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException($"Record is null.");
            }

            string output = "#";
            PropertyInfo[] properties = record.GetType().GetProperties();

            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].PropertyType == typeof(DateTime))
                {
                    DateTime date = (DateTime)properties[i].GetValue(record);
                    output += "Date of birth" + ": ";
                    output += date.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture);
                }
                else
                {
                    if (properties[i].Name != "Id")
                    {
                        output += properties[i].Name + ": ";
                    }

                    output += properties[i].GetValue(record);
                }

                if (i != properties.Length - 1)
                {
                    output += ", ";
                }
            }

            Console.WriteLine(output);
        }
    }
}
