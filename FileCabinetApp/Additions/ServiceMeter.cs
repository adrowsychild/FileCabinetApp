using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp
{
    /// <summary>
    /// Class for measuring timings of service's methods.
    /// </summary>
    public class ServiceMeter : IFileCabinetService
    {
        private IFileCabinetService service;

        private Stopwatch watch;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceMeter"/> class.
        /// </summary>
        /// <param name="service">Service for measuring.</param>
        public ServiceMeter(IFileCabinetService service)
        {
            this.service = service;
        }

        /// <summary>
        /// Adds timing count for Add method.
        /// </summary>
        /// <param name="record">Record to add.</param>
        /// <returns>Record's id.</returns>
        public int AddRecord(FileCabinetRecord record)
        {
            this.watch = Stopwatch.StartNew();
            int id = this.service.AddRecord(record);
            this.watch.Stop();
            Console.WriteLine($"Add method execution duration is " + this.watch.ElapsedTicks + " ticks.");

            return id;
        }

        /// <summary>
        /// Adds timing count for create method.
        /// </summary>
        /// <param name="record">Record to create.</param>
        /// <returns>Record's id.</returns>
        public int CreateRecord(FileCabinetRecord record)
        {
            this.watch = Stopwatch.StartNew();
            int id = this.service.CreateRecord(record);
            this.watch.Stop();
            Console.WriteLine($"Create method execution duration is " + this.watch.ElapsedTicks + " ticks.");

            return id;
        }

        /// <summary>
        /// Adds timing count for edit method.
        /// </summary>
        /// <param name="record">Record to edit.</param>
        /// <returns>Whether operation succeeded.</returns>
        public int EditRecord(FileCabinetRecord record)
        {
            this.watch = Stopwatch.StartNew();
            int id = this.service.EditRecord(record);
            this.watch.Stop();
            Console.WriteLine($"Edit method execution duration is " + this.watch.ElapsedTicks + " ticks.");

            return id;
        }

        /// <summary>
        /// Adds timing count for FindById method.
        /// </summary>
        /// <param name="id">Given id.</param>
        /// <returns>The array of records.</returns>
        public IEnumerable<FileCabinetRecord> FindById(string id)
        {
            this.watch = Stopwatch.StartNew();
            var foundRecords = this.service.FindById(id);
            this.watch.Stop();
            Console.WriteLine($"FindById method execution duration is " + this.watch.ElapsedTicks + " ticks.");

            return foundRecords;
        }

        /// <summary>
        /// Adds timing count for FindByFirstName method.
        /// </summary>
        /// <param name="firstName">Given first name.</param>
        /// <returns>The array of records.</returns>
        public IEnumerable<FileCabinetRecord> FindByFirstName(string firstName)
        {
            this.watch = Stopwatch.StartNew();
            var foundRecords = this.service.FindByFirstName(firstName);
            this.watch.Stop();
            Console.WriteLine($"FindByFirstName method execution duration is " + this.watch.ElapsedTicks + " ticks.");

            return foundRecords;
        }

        /// <summary>
        /// Adds timing count for FindByLastName method.
        /// </summary>
        /// <param name="lastName">Given last name.</param>
        /// <returns>The array of records.</returns>
        public IEnumerable<FileCabinetRecord> FindByLastName(string lastName)
        {
            this.watch = Stopwatch.StartNew();
            var foundRecords = this.service.FindByLastName(lastName);
            this.watch.Stop();
            Console.WriteLine($"FindByLastName method execution duration is " + this.watch.ElapsedTicks + " ticks.");

            return foundRecords;
        }

        /// <summary>
        /// Adds timing count for FindByDateOfBirth method.
        /// </summary>
        /// <param name="dateOfBirth">Given date of birth.</param>
        /// <returns>The array of records.</returns>
        public IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth)
        {
            this.watch = Stopwatch.StartNew();
            var foundRecords = this.service.FindByDateOfBirth(dateOfBirth);
            this.watch.Stop();
            Console.WriteLine($"FindByDateOfBirth method execution duration is " + this.watch.ElapsedTicks + " ticks.");

            return foundRecords;
        }

        /// <summary>
        /// Adds timing count for FindByFavouriteNumber method.
        /// </summary>
        /// <param name="favNumber">Given favourite number.</param>
        /// <returns>The array of records.</returns>
        public IEnumerable<FileCabinetRecord> FindByFavouriteNumber(string favNumber)
        {
            this.watch = Stopwatch.StartNew();
            var foundRecords = this.service.FindByFavouriteNumber(favNumber);
            this.watch.Stop();
            Console.WriteLine($"FindByFavouriteNumber method execution duration is " + this.watch.ElapsedTicks + " ticks.");

            return foundRecords;
        }

        /// <summary>
        /// Adds timing count for FindByFavouriteCharacter method.
        /// </summary>
        /// <param name="favChar">Given favourite character.</param>
        /// <returns>The array of records.</returns>
        public IEnumerable<FileCabinetRecord> FindByFavouriteCharacter(string favChar)
        {
            this.watch = Stopwatch.StartNew();
            var foundRecords = this.service.FindByFavouriteCharacter(favChar);
            this.watch.Stop();
            Console.WriteLine($"FindByFavouriteCharacter method execution duration is " + this.watch.ElapsedTicks + " ticks.");

            return foundRecords;
        }

        /// <summary>
        /// Adds timing count for FindByFavouriteGame method.
        /// </summary>
        /// <param name="favGame">Given favourite game.</param>
        /// <returns>The array of records.</returns>
        public IEnumerable<FileCabinetRecord> FindByFavouriteGame(string favGame)
        {
            this.watch = Stopwatch.StartNew();
            var foundRecords = this.service.FindByFavouriteGame(favGame);
            this.watch.Stop();
            Console.WriteLine($"FindByFavouriteGame method execution duration is " + this.watch.ElapsedTicks + " ticks.");

            return foundRecords;
        }

        /// <summary>
        /// Adds timing count for FindByDonations method.
        /// </summary>
        /// <param name="donations">Given donations.</param>
        /// <returns>The array of records.</returns>
        public IEnumerable<FileCabinetRecord> FindByDonations(string donations)
        {
            this.watch = Stopwatch.StartNew();
            var foundRecords = this.service.FindByDonations(donations);
            this.watch.Stop();
            Console.WriteLine($"FindByDonations method execution duration is " + this.watch.ElapsedTicks + " ticks.");

            return foundRecords;
        }

        /// <summary>
        /// Adds timing count for GetDeleted method.
        /// </summary>
        /// <returns>The number of deleted records.</returns>
        public int GetDeleted()
        {
            // this.watch = Stopwatch.StartNew();
            int deleted = this.service.GetDeleted();

            // this.watch.Stop();
            // Console.WriteLine($"GetDeleted method execution duration is " + this.watch.ElapsedTicks + " ticks.");
            // Console.WriteLine($"GetDeleted method execution duration is " + this.watch.ElapsedTicks + " ticks.");
            return deleted;
        }

        /// <summary>
        /// Adds timing count for GetIds method.
        /// </summary>
        /// <returns>List of ids in the list.</returns>
        public List<int> GetIds()
        {
            // this.watch = Stopwatch.StartNew();
            List<int> ids = this.service.GetIds();

            // this.watch.Stop();
            // Console.WriteLine($"GetIds method execution duration is " + this.watch.ElapsedTicks + " ticks.");
            return ids;
        }

        /// <summary>
        /// Adds timing count for GetRecords method.
        /// </summary>
        /// <returns>The array of records.</returns>
        public ReadOnlyCollection<FileCabinetRecord> GetRecords()
        {
            this.watch = Stopwatch.StartNew();
            ReadOnlyCollection<FileCabinetRecord> records = this.service.GetRecords();
            this.watch.Stop();
            Console.WriteLine($"GetRecords method execution duration is " + this.watch.ElapsedTicks + " ticks.");

            return records;
        }

        /// <summary>
        /// Adds timing count for GetStat method.
        /// </summary>
        /// <returns>The number of records.</returns>
        public int GetStat()
        {
            this.watch = Stopwatch.StartNew();
            int count = this.service.GetStat();
            this.watch.Stop();
            Console.WriteLine($"GetStat method execution duration is " + this.watch.ElapsedTicks + " ticks.");

            return count;
        }

        /// <summary>
        /// Adds timing count for GetValidator method.
        /// </summary>
        /// <returns>The service's validator.</returns>
        public IRecordValidator GetValidator()
        {
            // this.watch = Stopwatch.StartNew();
            var validator = this.service.GetValidator();

            // this.watch.Stop();
            // Console.WriteLine($"GetValidator method execution duration is " + this.watch.ElapsedTicks + " ticks.");
            return validator;
        }

        /// <summary>
        /// Adds timing count for MakeSnapshot method.
        /// </summary>
        /// <returns>>An instance of the IFileCabinetServiceSnapshot class.</returns>
        public IFileCabinetServiceSnapshot MakeSnapshot()
        {
            this.watch = Stopwatch.StartNew();
            var snapshot = this.service.MakeSnapshot();
            this.watch.Stop();
            Console.WriteLine($"MakeSnapshot method execution duration is " + this.watch.ElapsedTicks + " ticks.");

            return snapshot;
        }

        /// <summary>
        /// Adds timing count for RemoveRecord method.
        /// </summary>
        /// <param name="id">Id of record to remove.</param>
        /// <returns>Id of removed record if succeeded, -1 otherwise.</returns>
        public int RemoveRecord(int id)
        {
            this.watch = Stopwatch.StartNew();
            int removedId = this.service.RemoveRecord(id);
            this.watch.Stop();
            Console.WriteLine($"RemoveRecord method execution duration is " + this.watch.ElapsedTicks + " ticks.");

            return removedId;
        }

        /// <summary>
        /// Adds timing count for Restore method.
        /// </summary>
        /// <param name="snapshot">A snapshot of records to restore.</param>
        /// <returns>Number of imported records.</returns>
        public int Restore(IFileCabinetServiceSnapshot snapshot)
        {
            this.watch = Stopwatch.StartNew();
            int imported = this.service.Restore(snapshot);
            this.watch.Stop();
            Console.WriteLine($"Restore method execution duration is " + this.watch.ElapsedTicks + " ticks.");

            return imported;
        }
    }
}
