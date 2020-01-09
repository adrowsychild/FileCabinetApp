using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Xml;

namespace FileCabinetApp
{
    /// <summary>
    /// Saves information to xml file.
    /// </summary>
    public class FileCabinetRecordXmlWriter : IFileCabinetXmlWriter
    {
        private readonly XmlWriter writer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCabinetRecordXmlWriter"/> class.
        /// </summary>
        /// <param name="writer">Xml writer.</param>
        public FileCabinetRecordXmlWriter(XmlWriter writer)
        {
            this.writer = writer;
        }

        /// <summary>
        /// Writes information to xml file.
        /// </summary>
        /// <param name="record">Record to write about.</param>
        /// <returns>Whether operation succeeded.</returns>
        public bool Write(FileCabinetRecord record)
        {
            if (record == null)
            {
                return false;
            }

            this.writer.WriteStartElement("record");
            this.writer.WriteAttributeString("id", record.Id.ToString(CultureInfo.InvariantCulture));
            this.writer.WriteStartElement("name");
            this.writer.WriteAttributeString("first", record.FirstName);
            this.writer.WriteAttributeString("last", record.LastName);
            this.writer.WriteEndElement();
            this.writer.WriteStartElement("dateOfBirth");
            this.writer.WriteString(record.DateOfBirth.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture));
            this.writer.WriteEndElement();
            this.writer.WriteStartElement("favourite");
            this.writer.WriteAttributeString("number", record.FavouriteNumber.ToString(CultureInfo.InvariantCulture));
            this.writer.WriteAttributeString("character", record.FavouriteCharacter.ToString(CultureInfo.InvariantCulture));
            this.writer.WriteAttributeString("game", record.FavouriteGame);
            this.writer.WriteEndElement();
            this.writer.WriteStartElement("donations");
            this.writer.WriteString(record.Donations.ToString(CultureInfo.InvariantCulture));
            this.writer.WriteEndElement();
            this.writer.WriteEndElement();

            return true;
        }
    }
}
