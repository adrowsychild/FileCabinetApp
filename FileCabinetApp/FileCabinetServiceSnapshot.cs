using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Xml;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp
{
    /// <summary>
    /// Snapshot of records and methods saving them.
    /// </summary>
    public class FileCabinetServiceSnapshot : IFileCabinetServiceSnapshot
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        public FileCabinetServiceSnapshot()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">Records to snapshot.</param>
        public FileCabinetServiceSnapshot(ReadOnlyCollection<FileCabinetRecord> records)
        {
            this.Records = records;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetServiceSnapshot"/> class.
        /// </summary>
        /// <param name="records">Records to snapshot.</param>
        public FileCabinetServiceSnapshot(IList<FileCabinetRecord> records)
        {
            this.Records = new ReadOnlyCollection<FileCabinetRecord>(records);
        }

        /// <summary>
        /// Gets a snapshot of records.
        /// </summary>
        /// <value>A snapshot of records in the concrete moment.</value>
        public ReadOnlyCollection<FileCabinetRecord> Records { get; }

        /// <summary>
        /// Saves the records to csv file.
        /// </summary>
        /// <param name="writer">Csv writer.</param>
        /// <returns>Whether operation succeeded.</returns>
        public bool SaveToCsv(StreamWriter writer)
        {
            FileCabinetRecordCsvWriter csvWriter = new FileCabinetRecordCsvWriter(writer);
            foreach (var record in this.Records)
            {
                csvWriter.Write(record);
            }

            return true;
        }

        /// <summary>
        /// Saves the records to xml file.
        /// </summary>
        /// <param name="writer">Xml writer.</param>
        /// <returns>Whether operation succeeded.</returns>
        public bool SaveToXml(StreamWriter writer)
        {
            XmlWriter xmlWriter = XmlWriter.Create(writer);
            FileCabinetRecordXmlWriter fileXmlWriter = new FileCabinetRecordXmlWriter(xmlWriter);

            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("records");

            foreach (var record in this.Records)
            {
                fileXmlWriter.Write(record);
            }

            xmlWriter.WriteEndDocument();
            xmlWriter.Close();

            return true;
        }

        /// <summary>
        /// Loads the records from csv file.
        /// </summary>
        /// <param name="reader">Csv reader.</param>
        /// <returns>List of imported records.</returns>
        public IList<FileCabinetRecord> LoadFromCsv(StreamReader reader)
        {
            FileCabinetRecordCsvReader csvReader = new FileCabinetRecordCsvReader(reader);
            IList<FileCabinetRecord> records = csvReader.ReadAll();

            return records;
        }

        /// <summary>
        /// Loads the records from xml file.
        /// </summary>
        /// <param name="reader">Xml reader.</param>
        /// <returns>List of imported records.</returns>
        public IList<FileCabinetRecord> LoadFromXml(StreamReader reader)
        {
            XmlReader xmlReader = XmlReader.Create(reader);
            FileCabinetRecordXmlReader fileXmlReader = new FileCabinetRecordXmlReader(xmlReader);
            IList<FileCabinetRecord> records = fileXmlReader.ReadAll();

            return records;
        }
    }
}
