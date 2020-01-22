using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FileCabinetApp
{
    /// <summary>
    /// Enumerator for iterating through records.
    /// </summary>
    public class FileSystemRecordCollection : IEnumerable<FileCabinetRecord>
    {
        private FileCabinetFilesystemService service;
        private ReadOnlyCollection<FileCabinetRecord> records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemRecordCollection"/> class.
        /// </summary>
        /// <param name="service">Service whose records to iterate through.</param>
        /// <param name="records">Records to iterate through.</param>
        public FileSystemRecordCollection(FileCabinetFilesystemService service, ReadOnlyCollection<FileCabinetRecord> records)
        {
            this.service = service;
            this.records = records;
        }

        public static explicit operator List<FileCabinetRecord>(FileSystemRecordCollection enumeratedRecords)
        {
            if (enumeratedRecords == null)
            {
                return null;
            }

            List<FileCabinetRecord> records = new List<FileCabinetRecord>();
            foreach (var record in enumeratedRecords)
            {
                records.Add(record);
            }

            return records;
        }

        /// <summary>
        /// Converts the instance of enumerator to list.
        /// </summary>
        /// <param name="enumeratedRecords">Enumerator to convert.</param>
        /// <returns>List.</returns>
        public static List<FileCabinetRecord> ToList(FileSystemRecordCollection enumeratedRecords)
        {
            List<FileCabinetRecord> records = (List<FileCabinetRecord>)enumeratedRecords;
            return records;
        }

        /// <summary>
        /// Iterator to iterate through the records.
        /// </summary>
        /// <returns>Single record.</returns>
        public IEnumerator<FileCabinetRecord> GetEnumerator()
        {
            foreach (var record in this.records)
            {
                yield return record;
            }
        }

        /// <summary>
        /// Iterator to iterate through the records.
        /// </summary>
        /// <returns>Single record.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (var record in this.records)
            {
                yield return record;
            }
        }
    }
}
