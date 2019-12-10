namespace FileCabinetApp
{
    using System;
    using System.Collections.Generic;

    public class FileCabinetService
    {
        private readonly List<FileCabinetRecord> list = new List<FileCabinetRecord>();

        public int CreateRecord(string firstName, string lastName, DateTime dateOfBirth)
        {
            var record = new FileCabinetRecord
            {
                Id = this.list.Count + 1,
                FirstName = firstName,
                LastName = lastName,
                DateOfBirth = dateOfBirth,
            };

            this.list.Add(record);

            return record.Id;
        }

        public FileCabinetRecord[] GetRecords()
        {
            FileCabinetRecord[] listCopied = new FileCabinetRecord[this.list.Count];
            this.list.CopyTo(listCopied);
            return listCopied;
        }

        public int GetStat()
        {
            return this.list.Count;
        }
    }
}