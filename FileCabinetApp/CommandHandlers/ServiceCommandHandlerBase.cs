using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the user's commands which relate to the service.
    /// </summary>
    public abstract class ServiceCommandHandlerBase : CommandHandlerBase
    {
        /// <summary>
        /// The service to get information and modify.
        /// </summary>
        protected IFileCabinetService service;

        /// <summary>
        /// Shows one record.
        /// </summary>
        /// <param name="record">Record to show.</param>
        protected static void ShowRecord(FileCabinetRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException($"Record is null.");
            }

            string output = "#";
            PropertyInfo[] properties = record.GetType().GetProperties();

            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].PropertyType == typeof(DateTime))
                {
                    DateTime date = (DateTime)properties[i].GetValue(record);
                    output += "Date of birth" + ": ";
                    output += date.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture);
                }
                else
                {
                    if (properties[i].Name != "Id")
                    {
                        output += properties[i].Name + ": ";
                    }

                    output += properties[i].GetValue(record);
                }

                if (i != properties.Length - 1)
                {
                    output += ", ";
                }
            }

            Console.WriteLine(output);
        }

        /// <summary>
        /// Shows list of records to the user.
        /// </summary>
        /// <param name="records">Record to show.</param>
        protected static void ShowRecords(IReadOnlyCollection<FileCabinetRecord> records)
        {
            IEnumerable<FileCabinetRecord> orderedRecords = records.OrderBy(record => record.Id);
            foreach (var record in orderedRecords)
            {
                ShowRecord(record);
            }
        }

        /// <summary>
        /// Converts the value to specific type.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="input">The input to convert.</param>
        /// <returns>
        /// Bool whether the conversion succeeded.
        /// Type of conversion as string.
        /// Converted value itself.
        /// </returns>
        protected static Tuple<bool, string, T> Converter<T>(string input)
        {
            bool conversionSucceeded = false;
            string conversionType = $"from string to {typeof(T)}";
            T convertedValue = default;

            try
            {
                convertedValue = (T)Convert.ChangeType(input, typeof(T), CultureInfo.InvariantCulture);
                conversionSucceeded = true;
            }
            catch (FormatException)
            {
            }

            return new Tuple<bool, string, T>(conversionSucceeded, conversionType, convertedValue);
        }

        /// <summary>
        /// Validates the user's value.
        /// </summary>
        /// <typeparam name="T">The type of value to validate.</typeparam>
        /// <param name="validator">The specific validator.</param>
        /// <param name="field">The field to validate.</param>
        /// <param name="input">The value to validate.</param>
        /// <returns>
        /// Bool whether the validation succeeded.
        /// Type of conversion as string.
        /// </returns>
        protected static Tuple<bool, string> Validator<T>(IRecordValidator validator, string field, T input)
        {
            if (validator == null)
            {
                throw new ArgumentNullException($"Validator is null.");
            }

            bool validationSucceeded;
            string validationType = field + " field";
            Type validatorType = validator.GetType();
            MethodInfo validateField = validatorType.GetMethod("Validate" + field);

            validationSucceeded = (bool)validateField.Invoke(validator, new object[] { input });

            return new Tuple<bool, string>(validationSucceeded, validationType);
        }

        /// <summary>
        /// Requests and checks the user's input.
        /// </summary>
        /// <param name="id">Id to create the record with.</param>
        /// <returns>Valid record.</returns>
        protected FileCabinetRecord CheckRecordInput(int id = 1)
        {
            Console.WriteLine("First Name: ");
            string tmpFirstName = this.ReadInput(Converter<string>, Validator, "FirstName");

            Console.WriteLine("Last Name: ");
            string tmpLastName = this.ReadInput(Converter<string>, Validator, "LastName");

            Console.WriteLine("Date of Birth: ");
            DateTime tmpDateOfBirth = this.ReadInput(Converter<DateTime>, Validator, "DateOfBirth");

            Console.WriteLine("Favourite number: ");
            short tmpFavouriteNumber = this.ReadInput(Converter<short>, Validator, "FavouriteNumber");

            Console.WriteLine("Favourite character: ");
            char tmpFavouriteCharacter = this.ReadInput(Converter<char>, Validator, "FavouriteCharacter");

            Console.WriteLine("Favourite game: ");
            string tmpFavouriteGame = this.ReadInput(Converter<string>, Validator, "FavouriteGame");

            Console.WriteLine("Donations: ");
            decimal tmpDonations = this.ReadInput(Converter<decimal>, Validator, "Donations");

            FileCabinetRecord record = new FileCabinetRecord(id, tmpFirstName, tmpLastName, tmpDateOfBirth, tmpFavouriteNumber, tmpFavouriteCharacter, tmpFavouriteGame, tmpDonations);

            return record;
        }

        /// <summary>
        /// Parses and validates the user's input.
        /// </summary>
        /// <typeparam name="T">The type of input.</typeparam>
        /// <param name="converter">Converter from string to specific type.</param>
        /// <param name="validator">Validator for the converted input.</param>
        /// <param name="field">Field to check.</param>
        /// <returns>Converted and validated value.</returns>
        protected T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<IRecordValidator, string, T, Tuple<bool, string>> validator, string field)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                var validationResult = validator(this.service.GetValidator(), field, value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

                return value;
            }
            while (true);
        }
    }
}
