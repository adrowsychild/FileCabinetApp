using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp
{
    /// <summary>
    /// Interface for class for working with list of users.
    /// </summary>
    public interface IFileCabinetService
    {
        /// <summary>
        /// Creates a new record.
        /// </summary>
        /// <param name="record">User's info.</param>
        /// <returns>User's id in the users' list.</returns>
        int CreateRecord(FileCabinetRecord record);

        /// <summary>
        /// Edits the existing record.
        /// </summary>
        /// <param name="record">Record to edit.</param>
        /// <returns>Whether operation succeeded.</returns>
        int EditRecord(FileCabinetRecord record);

        /// <summary>
        /// Adds record to the list of records.
        /// </summary>
        /// <param name="record">Record to add.</param>
        /// <returns>Record's id.</returns>
        int AddRecord(FileCabinetRecord record);

        /// <summary>
        /// Removes a record from the list by given id.
        /// </summary>
        /// <param name="id">Id to remove record by.</param>
        /// <returns>Id of removed record if succeeded, -1 otherwise.
        /// </returns>
        int RemoveRecord(int id);

        /// <summary>
        /// Searches the records by id.
        /// </summary>
        /// <param name="id">Given id.</param>
        /// <returns>The array of records.</returns>
        IEnumerable<FileCabinetRecord> FindById(string id);

        /// <summary>
        /// Searches the records by first name.
        /// </summary>
        /// <param name="firstName">Given first name.</param>
        /// <returns>The array of records.</returns>
        IEnumerable<FileCabinetRecord> FindByFirstName(string firstName);

        /// <summary>
        /// Searches the records by last name.
        /// </summary>
        /// <param name="lastName">Given last name.</param>
        /// <returns>The array of records.</returns>
        IEnumerable<FileCabinetRecord> FindByLastName(string lastName);

        /// <summary>
        /// Searches the records by date of birth.
        /// </summary>
        /// <param name="dateOfBirth">Given date of birth.</param>
        /// <returns>The array of records.</returns>
        IEnumerable<FileCabinetRecord> FindByDateOfBirth(string dateOfBirth);

        /// <summary>
        /// Searches the records by favourite number.
        /// </summary>
        /// <param name="favNumber">Given favourite number.</param>
        /// <returns>The array of records.</returns>
        IEnumerable<FileCabinetRecord> FindByFavouriteNumber(string favNumber);

        /// <summary>
        /// Searches the records by favourite character.
        /// </summary>
        /// <param name="favCharacter">Given favourite character.</param>
        /// <returns>The array of records.</returns>
        IEnumerable<FileCabinetRecord> FindByFavouriteCharacter(string favCharacter);

        /// <summary>
        /// Searches the records by favourite game.
        /// </summary>
        /// <param name="favGame">Given favourite game.</param>
        /// <returns>The array of records.</returns>
        IEnumerable<FileCabinetRecord> FindByFavouriteGame(string favGame);

        /// <summary>
        /// Searches the records by donations.
        /// </summary>
        /// <param name="donations">Given donations.</param>
        /// <returns>The array of records.</returns>
        IEnumerable<FileCabinetRecord> FindByDonations(string donations);

        /// <summary>
        /// Makes a snapshot of records in the concrete moment.
        /// </summary>
        /// <returns>An instance of the IFileCabinetServiceSnapshot class.</returns>
        IFileCabinetServiceSnapshot MakeSnapshot();

        /// <summary>
        /// Restores the list of records by the snapshot.
        /// </summary>
        /// <param name="snapshot">Snapshot to restore by.</param>
        /// <returns>Number of imported records.</returns>
        int Restore(IFileCabinetServiceSnapshot snapshot);

        /// <summary>
        /// Gets all the records.
        /// </summary>
        /// <returns>The array of records.</returns>
        ReadOnlyCollection<FileCabinetRecord> GetRecords();

        /// <summary>
        /// Returns the number of records in the list.
        /// </summary>
        /// <returns>The number of records.</returns>
        int GetStat();

        /// <summary>
        /// Returns the number of deleted records in the list.
        /// </summary>
        /// <returns>The number of deleted records.</returns>
        int GetDeleted();

        /// <summary>
        /// Returns list of ids in the list.
        /// </summary>
        /// <returns>List of ids in the list.</returns>
        List<int> GetIds();

        /// <summary>
        /// Gets the validator.
        /// </summary>
        /// <returns>Validator.</returns>
        IRecordValidator GetValidator();
    }
}
