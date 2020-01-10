using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FileCabinetApp.Validators.DefaultValidator;

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
        protected Tuple<bool, string> Validator<T>(IRecordValidator validator, string field, T input)
        {
            if (validator == null)
            {
                throw new ArgumentNullException($"Validator is null.");
            }

            bool validationSucceeded;
            string validationType = field + " field";
            string validatorType = this.service.GetValidatorType();
            validatorType = validatorType.First().ToString(CultureInfo.InvariantCulture).ToUpper() + validatorType.Substring(1);

            var asm = Assembly.Load("FileCabinetApp");
            var partValidatorType = asm.GetTypes().Where(v => v.Name.Contains(field, StringComparison.InvariantCulture) && v.Name.Contains(validatorType.ToString(CultureInfo.InvariantCulture), StringComparison.InvariantCulture)).ToList();
            MethodInfo validateMethod = partValidatorType[0].GetMethod("Validate");
            object classInstance = Activator.CreateInstance(partValidatorType[0], null);
            validationSucceeded = (bool)validateMethod.Invoke(classInstance, new object[] { input });

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
