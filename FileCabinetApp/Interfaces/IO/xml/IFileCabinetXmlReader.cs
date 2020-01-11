using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// Loads information from xml file.
    /// </summary>
    public interface IFileCabinetXmlReader
    {
        /// <summary>
        /// Reads records from xml file.
        /// </summary>
        /// <returns>List of records.</returns>
        public IList<FileCabinetRecord> ReadAll();
    }
}
