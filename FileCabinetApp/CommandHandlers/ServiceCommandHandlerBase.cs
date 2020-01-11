using System;
using System.Globalization;

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
        /// Parses and validates the user's input.
        /// </summary>
        /// <typeparam name="T">The type of input.</typeparam>
        /// <param name="converter">Converter from string to specific type.</param>
        /// <returns>Converted and validated value.</returns>
        protected static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter)
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

                return value;
            }
            while (true);
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
        /// Requests and checks the user's input.
        /// </summary>
        /// <param name="id">Id to create the record with.</param>
        /// <returns>Valid record.</returns>
        protected FileCabinetRecord CheckRecordInput(int id = 1)
        {
            FileCabinetRecord record = null;
            string exceptionMessage = null;

            do
            {
                Console.WriteLine("First Name: ");
                string tmpFirstName = ReadInput(Converter<string>);

                Console.WriteLine("Last Name: ");
                string tmpLastName = ReadInput(Converter<string>);

                Console.WriteLine("Date of Birth: ");
                DateTime tmpDateOfBirth = ReadInput(Converter<DateTime>);

                Console.WriteLine("Favourite number: ");
                short tmpFavouriteNumber = ReadInput(Converter<short>);

                Console.WriteLine("Favourite character: ");
                char tmpFavouriteCharacter = ReadInput(Converter<char>);

                Console.WriteLine("Favourite game: ");
                string tmpFavouriteGame = ReadInput(Converter<string>);

                Console.WriteLine("Donations: ");
                decimal tmpDonations = ReadInput(Converter<decimal>);

                record = new FileCabinetRecord(id, tmpFirstName, tmpLastName, tmpDateOfBirth, tmpFavouriteNumber, tmpFavouriteCharacter, tmpFavouriteGame, tmpDonations);
                exceptionMessage = this.service.GetValidator().Validate(record);
                if (exceptionMessage != null)
                {
                    Console.WriteLine("Validation failed:");
                    Console.WriteLine(exceptionMessage);
                    Console.WriteLine("Please Try again.");
                }
            }
            while (exceptionMessage != null);

            return record;
        }
    }
}
