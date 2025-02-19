﻿using System;
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
        ReadOnlyCollection<FileCabinetRecord> Records { get; }

        /// <summary>
        /// Saves the records to csv file.
        /// </summary>
        /// <param name="writer">Writer.</param>
        /// <returns>Whether operation succeeded.</returns>
        bool SaveToCsv(StreamWriter writer);

        /// <summary>
        /// Saves the records to xml file.
        /// </summary>
        /// <param name="writer">Writer.</param>
        /// <returns>Whether operation succeeded.</returns>
        bool SaveToXml(StreamWriter writer);

        /// <summary>
        /// Loads the records from csv file.
        /// </summary>
        /// <param name="reader">Csv reader.</param>
        /// <returns>Whether operation succeeded.</returns>
        IList<FileCabinetRecord> LoadFromCsv(StreamReader reader);

        /// <summary>
        /// Loads the records from xml file.
        /// </summary>
        /// <param name="reader">Xml reader.</param>
        /// <returns>List of imported records.</returns>
        IList<FileCabinetRecord> LoadFromXml(StreamReader reader);
    }
}
