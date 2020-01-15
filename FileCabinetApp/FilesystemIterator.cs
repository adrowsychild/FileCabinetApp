using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using FileCabinetApp.Helpers;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp
{
    public class FilesystemIterator : IRecordIterator
    {
        private int currentRecord = -1;
        private FileCabinetFilesystemService service;
        private ReadOnlyCollection<FileCabinetRecord> records;

        public FilesystemIterator(FileCabinetFilesystemService service, ReadOnlyCollection<FileCabinetRecord> records)
        {
            this.service = service;
            this.records = records;
        }

        public FileCabinetRecord this[int index] { get => this.records[index]; }

    public int Count()
        {
            return this.records.Count;
        }

        public FileCabinetRecord GetNext()
        {
            FileCabinetRecord record = null;

            if (this.HasMore())
            {
                record = this.service.ReadRecord(this.currentRecord);
                this.currentRecord += FileCabinetFilesystemService.RecordSize;
            }

            return record;
        }

        public bool HasMore()
        {
            if (this.currentRecord + 1 == this.records.Count)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
