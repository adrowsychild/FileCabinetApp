using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using FileCabinetApp.Interfaces;
using static FileCabinetApp.Program;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// General handler for the user's commands.
    /// </summary>
    public class CommandHandler : CommandHandlerBase
    {
        private const int COMMANDHELPINDEX = 0;
        private const int DESCRIPTIONHELPINDEX = 1;
        private const int EXPLANATIONHELPINDEX = 2;

        private static readonly Tuple<string, Action<string>>[] Commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
            new Tuple<string, Action<string>>("export", Export),
            new Tuple<string, Action<string>>("import", Import),
            new Tuple<string, Action<string>>("remove", Remove),
            new Tuple<string, Action<string>>("purge", Purge),
        };

        private static readonly string[][] HelpMessages = new string[][]
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

        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <param name="request">Request to handle.</param>
        /// <returns>The object back, or null otherwise.</returns>
        public override object Handle(AppCommandRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException($"Request in null.");
            }

            // MethodInfo method = typeof(CommandHandler).GetMethod(request.Command, BindingFlags.NonPublic | BindingFlags.Static);
            // method.Invoke(null, new object[] { request.Parameters });
            var index = Array.FindIndex(Commands, 0, Commands.Length, i => i.Item1.Equals(request.Command, StringComparison.InvariantCultureIgnoreCase));
            if (index >= 0)
            {
                Commands[index].Item2(request.Parameters);
            }
            else
            {
                PrintMissedCommandInfo(request.Command);
            }

            return base.Handle(request);
        }

        /// <summary>
        /// Creates a new record.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        private static void Create(string parameters)
        {
            int id = FileCabinetService.CreateRecord(CheckRecordInput());
            Console.WriteLine($"Record #{id} is created.");
        }

        /// <summary>
        /// Edits the existing record.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        private static void Edit(string parameters)
        {
            if (int.TryParse(parameters, out int id))
            {
                if (id < 1 || !FileCabinetService.GetIds().Contains(id))
                {
                    Console.WriteLine($"#{id} record is not found.");
                    return;
                }
                else
                {
                    FileCabinetService.EditRecord(CheckRecordInput(id));
                    Console.WriteLine($"Record #{id} is updated.");
                }
            }
        }

        /// <summary>
        /// Removes the existing record.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        private static void Remove(string parameters)
        {
            if (int.TryParse(parameters, out int id))
            {
                if (id < 1 || !FileCabinetService.GetIds().Contains(id))
                {
                    Console.WriteLine($"#{id} record is not found.");
                    return;
                }
                else
                {
                    FileCabinetService.RemoveRecord(id);
                    Console.WriteLine($"Record #{id} is removed.");
                }
            }
        }

        /// <summary>
        /// Purges the list of records, if fileSystemService.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        private static void Purge(string parameters)
        {
            if (FileCabinetService.GetType() == typeof(FileCabinetFilesystemService))
            {
                int initialNumOfRecords = FileCabinetService.GetStat();

                MethodInfo method = typeof(FileCabinetFilesystemService).GetMethod("Purge");
                int recordsPurged = (int)method.Invoke(FileCabinetService, null);
                Console.WriteLine("Data file processing is completed: " + recordsPurged + " of " + initialNumOfRecords + " records were purged.");
            }
        }

        /// <summary>
        /// Searches for the record by key.
        /// </summary>
        /// <param name="parameters">Parameters to search by.</param>
        private static void Find(string parameters)
        {
            string[] args = parameters.Split();
            if (args.Length > 2)
            {
                return;
            }

            IReadOnlyCollection<FileCabinetRecord> foundRecords;

            try
            {
                switch (args[0].ToLower())
                {
                    case "firstname":
                        foundRecords = FileCabinetService.FindByFirstName(args[1]);
                        ShowRecords(foundRecords);
                        break;

                    case "lastname":
                        foundRecords = FileCabinetService.FindByLastName(args[1]);
                        ShowRecords(foundRecords);
                        break;

                    case "dateofbirth":
                        foundRecords = FileCabinetService.FindByDateOfBirth(args[1]);
                        ShowRecords(foundRecords);
                        break;
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Exports the records to csv or xml format.
        /// </summary>
        /// <param name="parameters">Format to write in and path to write to.</param>
        private static void Export(string parameters)
        {
            string[] args = parameters.Split();
            if (args == null || args.Length < 2)
            {
                Console.WriteLine("Incorrect parameters.");
                return;
            }

            string format = args[0].ToLower();
            string path = args[1];

            if (File.Exists(path))
            {
                Console.WriteLine("File is exist - rewrite " + path + "? [Y/n]");
                if (Console.ReadLine().ToLower() != "y")
                {
                    return;
                }
            }

            using (StreamWriter writer = new StreamWriter(path))
            {
                IFileCabinetServiceSnapshot snapshot = FileCabinetService.MakeSnapshot();
                bool isSucceed = false;
                switch (format)
                {
                    case "csv":
                        isSucceed = snapshot.SaveToCsv(writer);
                        break;

                    case "xml":
                        isSucceed = snapshot.SaveToXml(writer);
                        break;

                    default:
                        Console.WriteLine("Incorrect format: can be xml or csv.");
                        return;
                }

                if (!isSucceed)
                {
                    Console.WriteLine("Export failed: can't open file " + path);
                }
                else
                {
                    Console.WriteLine("All records are exported to file " + path);
                }
            }
        }

        /// <summary>
        /// Imports the records from csv or xml file.
        /// </summary>
        /// <param name="parameters">Format to write in and path to write to.</param>
        private static void Import(string parameters)
        {
            string[] args = parameters.Split();
            if (args == null || args.Length < 2)
            {
                Console.WriteLine("Incorrect parameters.");
                return;
            }

            string format = args[0].ToLower();
            string path = args[1];

            if (!File.Exists(path))
            {
                Console.WriteLine("Import error: " + path + " does not exist");
                return;
            }

            using (StreamReader reader = new StreamReader(path))
            {
                int importedRecords = 0;
                IList<FileCabinetRecord> records;
                IFileCabinetServiceSnapshot snapshot = new FileCabinetServiceSnapshot();
                switch (format)
                {
                    case "csv":
                        records = snapshot.LoadFromCsv(reader);
                        snapshot = new FileCabinetServiceSnapshot(records);
                        importedRecords = FileCabinetService.Restore(snapshot);
                        break;

                    case "xml":
                        records = snapshot.LoadFromXml(reader);
                        snapshot = new FileCabinetServiceSnapshot(records);
                        importedRecords = FileCabinetService.Restore(snapshot);
                        break;

                    default:
                        Console.WriteLine("Incorrect format: can be xml or csv.");
                        return;
                }

                Console.WriteLine(importedRecords + " records were imported from " + path + ".");
            }
        }

        /// <summary>
        /// Shows all the records to the user.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        private static void List(string parameters)
        {
            ShowRecords(FileCabinetService.GetRecords());
        }

        /// <summary>
        /// Shows the number of records to the user.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        private static void Stat(string parameters)
        {
            var recordsCount = FileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
            Console.WriteLine($"{FileCabinetService.GetDeleted()} records are ready to be purged.");
        }

        /// <summary>
        /// Shows one record.
        /// </summary>
        /// <param name="record">Record to show.</param>
        private static void ShowRecord(FileCabinetRecord record)
        {
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
        private static void ShowRecords(IReadOnlyCollection<FileCabinetRecord> records)
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
        private static FileCabinetRecord CheckRecordInput(int id = 1)
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
        private static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter, Func<IRecordValidator, string, T, Tuple<bool, string>> validator, string field)
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
        private static Tuple<bool, string, T> Converter<T>(string input)
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
        private static Tuple<bool, string> Validator<T>(IRecordValidator validator, string field, T input)
        {
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
        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

        /// <summary>
        /// Prints help message to the user.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        private static void PrintHelp(string parameters)
        {
            if (!string.IsNullOrEmpty(parameters))
            {
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[COMMANDHELPINDEX], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(HelpMessages[index][EXPLANATIONHELPINDEX]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in HelpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[COMMANDHELPINDEX], helpMessage[DESCRIPTIONHELPINDEX]);
                }
            }

            Console.WriteLine();
        }

        /// <summary>
        /// Exits the application.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            FileCabinetService.Close();
            IsRunning = false;
        }
    }
}
