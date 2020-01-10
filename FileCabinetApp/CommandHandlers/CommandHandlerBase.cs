using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using static FileCabinetApp.Program;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the user's commands.
    /// </summary>
    public abstract class CommandHandlerBase : ICommandHandler
    {
        /// <summary>
        /// Index of command's name in HelpMessages table.
        /// </summary>
        protected const int COMMANDHELPINDEX = 0;

        /// <summary>
        /// Index of command's description in HelpMessages table.
        /// </summary>
        protected const int DESCRIPTIONHELPINDEX = 1;

        /// <summary>
        /// Index of command's explanation in HelpMessages table.
        /// </summary>
        protected const int EXPLANATIONHELPINDEX = 2;

        /// <summary>
        /// Prints the help messages to the user.
        /// </summary>
        protected static readonly string[][] HelpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints the number of records", "The 'stat' command prints the number of records." },
            new string[] { "create", "creates a new record", "The 'create' command creates a new record." },
            new string[] { "edit", "edits the existing record", "The 'edit' command edits the existing record." },
            new string[] { "find", "finds records by given criteria", "The 'find' command finds records by given criteria" },
            new string[] { "list", "prints the records", "The 'list' prints the records" },
            new string[] { "export", "exports the records to the file in xml or csv format", "The 'export' command exports the records to the file in xml or csv format." },
            new string[] { "import", "imports the records from the xml or csv file", "The 'import' command imports the records from the xml or csv file." },
            new string[] { "remove", "removes the record from the list", "The 'remove' command removes the record from the list" },
            new string[] { "purge", "purges the deleted records from the list", "The 'purge' command purges the deleted records from the list." },
        };

        private ICommandHandler nextHandler;

        /// <summary>
        /// Sets the next handler.
        /// </summary>
        /// <param name="handler">Handler to set.</param>
        /// <returns>Handler back.</returns>
        public ICommandHandler SetNext(ICommandHandler handler)
        {
            this.nextHandler = handler;

            return handler;
        }

        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <param name="request">Request to handle.</param>
        /// <returns>The object back, or null otherwise.</returns>
        public virtual object Handle(AppCommandRequest request)
        {
            if (this.nextHandler != null)
            {
                return this.nextHandler.Handle(request);
            }
            else
            {
                return null;
            }
        }

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
        /// Requests and checks the user's input.
        /// </summary>
        /// <param name="id">Id to create the record with.</param>
        /// <returns>Valid record.</returns>
        protected static FileCabinetRecord CheckRecordInput(int id = 1)
        {
            Console.WriteLine("First Name: ");
            string tmpFirstName = ReadInput(Converter<string>, Validator, "FirstName");

            Console.WriteLine("Last Name: ");
            string tmpLastName = ReadInput(Converter<string>, Validator, "LastName");

            Console.WriteLine("Date of Birth: ");
            DateTime tmpDateOfBirth = ReadInput(Converter<DateTime>, Validator, "DateOfBirth");

            Console.WriteLine("Favourite number: ");
            short tmpFavouriteNumber = ReadInput(Converter<short>, Validator, "FavouriteNumber");

            Console.WriteLine("Favourite character: ");
            char tmpFavouriteCharacter = ReadInput(Converter<char>, Validator, "FavouriteCharacter");

            Console.WriteLine("Favourite game: ");
            string tmpFavouriteGame = ReadInput(Converter<string>, Validator, "FavouriteGame");

            Console.WriteLine("Donations: ");
            decimal tmpDonations = ReadInput(Converter<decimal>, Validator, "Donations");

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
        protected static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<IRecordValidator, string, T, Tuple<bool, string>> validator, string field)
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

                var validationResult = validator(FileCabinetService.GetValidator(), field, value);
                if (!validationResult.Item1)
                {
                    Console.WriteLine($"Validation failed: {validationResult.Item2}. Please, correct your input.");
                    continue;
                }

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
        /// Notifies there's no such command.
        /// </summary>
        /// <param name="command">Incorrect command.</param>
        protected static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }
    }
}
