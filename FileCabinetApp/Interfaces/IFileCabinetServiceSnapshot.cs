using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// Snapshot of records and methods saving them.
    /// </summary>
    public interface IFileCabinetServiceSnapshot
    {
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
        public bool SaveToCsv(StreamWriter writer);

        //public book SaveToXml(StreamWriter writer);
    }
}
