using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FileCabinetApp
{
    /// <summary>
    /// Enumerator for iterating through records.
    /// </summary>
    public class FileSystemRecordEnumerator : IEnumerable<FileCabinetRecord>
    {
        private FileCabinetFilesystemService service;
        private ReadOnlyCollection<FileCabinetRecord> records;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemRecordEnumerator"/> class.
        /// </summary>
        /// <param name="service">Service whose records to iterate through.</param>
        /// <param name="records">Records to iterate through.</param>
        public FileSystemRecordEnumerator(FileCabinetFilesystemService service, ReadOnlyCollection<FileCabinetRecord> records)
        {
            this.service = service;
            this.records = records;
        }

        public static explicit operator List<FileCabinetRecord>(FileSystemRecordEnumerator enumeratedRecords)
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();
            foreach (var record in enumeratedRecords)
            {
                records.Add(record);
            }

            return records;
        }

        public static List<FileCabinetRecord> ToList(FileSystemRecordEnumerator enumeratedRecords)
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
