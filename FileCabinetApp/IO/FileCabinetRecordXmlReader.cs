using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp
{
    /// <summary>
    /// Loads information from xml file.
    /// </summary>
    internal class FileCabinetRecordXmlReader : IFileCabinetXmlReader
    {
        private readonly XmlReader reader;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlReader"/> class.
        /// </summary>
        /// <param name="reader">Xml writer.</param>
        public FileCabinetRecordXmlReader(XmlReader reader)
        {
            this.reader = reader;
        }

        /// <summary>
        /// Reads records from csv file.
        /// </summary>
        /// <returns>List of records.</returns>
        public IList<FileCabinetRecord> ReadAll()
        {
            IList<FileCabinetRecord> records = new List<FileCabinetRecord>();

            // this.reader.ReadStartElement("records");

            XmlSerializer ser = new XmlSerializer(typeof(List<FileCabinetRecord>));

            records = (IList<FileCabinetRecord>)ser.Deserialize(this.reader);

            /*
            while (this.reader.EOF != true)
            {
                FileCabinetRecord record = this.Read();
                if (record != null)
                {
                    records.Add(record);
                }
                else
                {
                    this.reader.ReadEndElement();
                }
            }*/

            return records;
        }

        /// <summary>
        /// Reades a single record from xml file.
        /// </summary>
        /// <returns>Record.</returns>
        public FileCabinetRecord Read()
        {
            if (this.reader.GetAttribute("id") == null)
            {
                return null;
            }

            int id = int.Parse(this.reader.GetAttribute("id"), CultureInfo.InvariantCulture);
            this.reader.ReadStartElement();
            string firstName = this.reader.GetAttribute("first");
            string lastName = this.reader.GetAttribute("last");
            this.reader.ReadStartElement();
            DateTime dateOfBirth = DateTime.ParseExact(this.reader.ReadElementContentAsString(), "yyyy-MMM-d", CultureInfo.InvariantCulture);
            short favNumber = short.Parse(this.reader.GetAttribute("number"), CultureInfo.InvariantCulture);
            char favCharacter = this.reader.GetAttribute("character")[0];
            string favGame = this.reader.GetAttribute("game");
            this.reader.ReadStartElement();
            decimal donations = this.reader.ReadElementContentAsDecimal();
            this.reader.ReadEndElement();

            return new FileCabinetRecord(id, firstName, lastName, dateOfBirth, favNumber, favCharacter, favGame, donations);
        }
    }
}
