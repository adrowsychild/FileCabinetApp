using System;
using System.IO;

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// Saves information to csv file.
    /// </summary>
    public interface IFileCabinetRecordCsvWriter
    {
        /// <summary>
        /// Writes information to csv file.
        /// </summary>
        /// <param name="record">Record to write about.</param>
        /// <returns>Whether operation succeeded.</returns>
        bool Write(FileCabinetRecord record);
    }
}
