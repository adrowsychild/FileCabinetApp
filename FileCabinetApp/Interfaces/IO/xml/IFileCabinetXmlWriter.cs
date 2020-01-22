using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp
{
    /// <summary>
    /// Saves information to xml file.
    /// </summary>
    public interface IFileCabinetXmlWriter
    {
        /// <summary>
        /// Writes information to xml file.
        /// </summary>
        /// <param name="record">Record to write about.</param>
        /// <returns>Whether operation succeeded.</returns>
        bool Write(FileCabinetRecord record);
    }
}
