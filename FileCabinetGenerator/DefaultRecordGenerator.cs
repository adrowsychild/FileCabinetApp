using FileCabinetApp;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetGenerator
{
    /// <summary>
    /// Record-generator with default records.
    /// </summary>
    public class DefaultRecordGenerator : IRecordGenerator
    {
        /// <summary>
        /// Generates a single record with the given id.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Generated record.</returns>
        public FileCabinetRecord GenerateRecord(int id)
        {
            var rand = new Random();

            return new FileCabinetRecord
            {
                Id = id,
                FirstName = GenerateString(rand, rand.Next(0, 60)),
                LastName = GenerateString(rand, rand.Next(0, 60)),
                DateOfBirth = new DateTime(rand.Next(1950, DateTime.Now.Year), rand.Next(1, 12), rand.Next(1, 28)),
                FavouriteNumber = (short)rand.Next(0, 100),
                FavouriteCharacter = (char)rand.Next(97, 122),
                FavouriteGame = GenerateString(rand, rand.Next(0, 60)),
                Donations = (decimal)rand.Next(0, 100),
            };
        }

        /// <summary>
        /// Generates the list of records of given amount, starting with given id. 
        /// </summary>
        /// <param name="startId">Start-id for records.</param>
        /// <param name="amount">Amount of records to generate.</param>
        /// <returns>List of generated records.</returns>
        public List<FileCabinetRecord> GenerateRecords(int startId, int amount)
        {
            List<FileCabinetRecord> records = new List<FileCabinetRecord>();

            for (int i = 0; i < amount; i++)
            {
                records.Add(GenerateRecord(startId + i));
            }

            return records;
        }

        private string GenerateString(Random random, int length)
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var stringChars = new char[length];

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);
        }
    }
}
