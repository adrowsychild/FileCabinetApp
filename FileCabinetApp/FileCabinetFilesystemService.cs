using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp
{
    public class FileCabinetFilesystemService : IFileCabinetService
    {
        File
        public int CreateRecord(FileCabinetRecord record)
        {
            throw new NotImplementedException();
        }

        public int EditRecord(FileCabinetRecord record)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<FileCabinetRecord> FindByFirstName(string firstName)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<FileCabinetRecord> FindByLastName(string lastName)
        {
            throw new NotImplementedException();
        }

        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            throw new NotImplementedException();
        }

        public int GetStat()
        {
            throw new NotImplementedException();
        }

        public IRecordValidator GetValidator()
        {
            throw new NotImplementedException();
        }

        public string GetValidatorType()
        {
            throw new NotImplementedException();
        }

        public IFileCabinetServiceSnapshot MakeSnapshot()
        {
            throw new NotImplementedException();
        }
    }
}
