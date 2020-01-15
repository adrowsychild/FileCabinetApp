using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Interfaces
{
    /// <summary>
    /// Iterator for iterating over records.
    /// </summary>
    public interface IRecordIterator
    {
        /// <summary>
        /// Gets the next record from the list.
        /// </summary>
        /// <returns>A single record.</returns>
        FileCabinetRecord GetNext();

        /// <summary>
        /// Whether collection has more records.
        /// </summary>
        /// <returns>Bool whether collection has more records.</returns>
        bool HasMore();

        int Count();

        FileCabinetRecord this[int index] { get; }
    }
}
