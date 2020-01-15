using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp
{
    public class MemoryIterator : IRecordIterator
    {
        private int position = -1;
        private ReadOnlyCollection<FileCabinetRecord> records;

        public MemoryIterator(ReadOnlyCollection<FileCabinetRecord> records)
        {
            this.records = records;
        }

        public FileCabinetRecord this[int index] { get => this.records[index]; }

        public int Count()
        {
            return this.records.Count;
        }

        public FileCabinetRecord GetNext()
        {
            if (this.HasMore())
            {
                return this.records[++this.position];
            }
            else
            {
                return null;
            }
        }

        public bool HasMore()
        {
            if (this.position + 1 == this.records.Count)
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
