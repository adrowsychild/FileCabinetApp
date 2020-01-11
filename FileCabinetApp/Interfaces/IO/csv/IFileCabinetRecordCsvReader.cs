using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// Loads information from csv file.
    /// </summary>
    public interface IFileCabinetRecordCsvReader
    {
        /// <summary>
        /// Reads records from csv file.
        /// </summary>
        /// <returns>List of records.</returns>
        public IList<FileCabinetRecord> ReadAll();
    }
}
