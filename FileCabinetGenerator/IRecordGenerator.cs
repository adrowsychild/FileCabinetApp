using FileCabinetApp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Record-generator.
    /// </summary>
    public interface IRecordGenerator
    {
        /// <summary>
        /// Generates a single record with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Generated record.</returns>
        FileCabinetRecord GenerateRecord(int id);

        /// <summary>
        /// Generates the list of records of given amount, starting with given id. 
        /// </summary>
        /// <param name="startId">Start-id for records.</param>
        /// <param name="amount">Amount of records to generate.</param>
        /// <returns>List of generated records.</returns>
        List<FileCabinetRecord> GenerateRecords(int startId, int amount);
    }
}
