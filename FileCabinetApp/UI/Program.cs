using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp
{
    /// <summary>
    /// Class for handling user's commands.
    /// </summary>
    public static class Program
    {
        private const string DEVELOPERNAME = "Dyl Aliaksandra";
        private const string HINTMESSAGE = "Enter your command, or enter 'help' to get help.";
        private const string INTRO = "File Cabinet Application, developed by " + DEVELOPERNAME;
        private const string FILENAME = "cabinet-records.db";
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
        };

        private static readonly string[][] HelpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints the number of records", "The 'stat' command prints the number of records." },
            new string[] { "create", "creates a new record", "The 'create' command creates a new record." },
            new string[] { "edit", "edits the existing record", "The 'edit' command edits the existing record." },
            new string[] { "find", "finds records by given criteria", "The 'find' command finds records by given criteria" },
            new string[] { "list", "prints the records", "The 'list' prints the records." },
            new string[] { "export", "exports the records to the file in xml or csv format.", "The 'export' command exports the records to the file in xml or csv format." },
            new string[] { "import", "imports the records from the xml or csv file.", "The 'import' command imports the records from the xml or csv file." },
        };

        /// <summary>
        /// Contains the name of operation and corresponding operation itself.
        /// Then, if we need to add settings, we only need to add values to the dictionary.
        /// Warning: Both forms of command should be added one after another.
        /// </summary>
        private static readonly Dictionary<string, Settings> ChangeSettings = new Dictionary<string, Settings>
        {
            ["-v"] = new Settings(SetValidationRules),
            ["--validation-rules"] = new Settings(SetValidationRules),
            ["-s"] = new Settings(SetStorageRules),
            ["--storage"] = new Settings(SetStorageRules),
        };

        private static bool isRunning = true;

        private static IFileCabinetService fileCabinetService;

        private static IRecordValidator validator;

        private delegate void Settings(string args);

        /// <summary>
        /// Method for handling user's commands.
        /// </summary>
        /// <param name="args">Additional rule-changing arguments.</param>
        public static void Main(string[] args)
        {
            string[] tempArgs = new string[2];

            SetDefaultSettings();

            if (args != null && args.Length > 0)
            {
                for (int i = 1; i <= args.Length;)
                {
                    tempArgs[0] = args[i - 1];
                    if (i != args.Length)
                    {
                        tempArgs[1] = args[i];
                    }

                    int parsedSetting = SettingsParser(tempArgs);
                    if (parsedSetting == 0)
                    {
                        break;
                    }
                    else if (parsedSetting == 1)
                    {
                        i++;
                    }
                    else if (parsedSetting == 2)
                    {
                        i += 2;
                    }
                }
            }

            Console.WriteLine(Program.INTRO);

            Console.WriteLine("Using " + fileCabinetService.GetValidatorType() + " validation rules.");
            Console.WriteLine("Using " + fileCabinetService.GetType().ToString()[26..fileCabinetService.GetType().ToString().IndexOf("Service", StringComparison.InvariantCulture)].ToLower() + " service type.");

            Console.WriteLine(Program.HINTMESSAGE);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HINTMESSAGE);
                    continue;
                }

                var index = Array.FindIndex(Commands, 0, Commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    Commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
        }

        /// <summary>
        /// Parses the settings from application arguments
        /// and searches for given operation in the dictionary.
        /// </summary>
        /// <param name="args">Arguments to parse.</param>
        private static int SettingsParser(string[] args)
        {
            string operation = string.Empty;
            string parameter = string.Empty;
            Settings makeChanges = null;
            int parsedSetting = 0;

            // --some-operation=paramater
            if (args[0].StartsWith("--", StringComparison.InvariantCulture))
            {
                int index = args[0].IndexOf("=", StringComparison.InvariantCulture);
                if (index != -1)
                {
                    operation = args[0].Substring(0, index);
                    parameter = args[0].Substring(index + 1);
                    if (ChangeSettings.ContainsKey(operation.ToLower()))
                    {
                        makeChanges = ChangeSettings[operation.ToLower()];
                        parsedSetting = 1;
                    }
                }
            }

            // -o parameter
            else if (args[0].StartsWith("-", StringComparison.InvariantCulture))
            {
                operation = args[0];
                if (args[1] != null)
                {
                    parameter = args[1];
                }

                if (ChangeSettings.ContainsKey(operation.ToLower()))
                {
                    makeChanges = ChangeSettings[operation.ToLower()];
                    parsedSetting = 2;
                }
            }

            if (!string.IsNullOrEmpty(operation) && !string.IsNullOrEmpty(parameter) && makeChanges != null)
            {
                makeChanges.Invoke(parameter.ToLower());
            }

            return parsedSetting;
        }

        /// <summary>
        /// Sets the default settings of the application.
        /// </summary>
        private static void SetDefaultSettings()
        {
            Settings setSettings = null;
            int i = 0;

            foreach (Settings op in ChangeSettings.Values)
            {
                // as there're two forms of writing commands and we only need one of them
                if (i % 2 == 0)
                {
                    setSettings += op;
                    i++;
                }
                else
                {
                    i++;
                    continue;
                }
            }

            setSettings.Invoke("default");
        }

        /// <summary>
        /// Sets the validation rules based on the parameter.
        /// </summary>
        /// <param name="validationRules">Validation rules to set.</param>
        private static void SetValidationRules(string validationRules)
        {
            validator = validationRules switch
            {
                "custom" => new CustomValidator(),
                _ => new DefaultValidator(),
            };
        }

        /// <summary>
        /// Sets the storage rules based on the parameter.
        /// </summary>
        /// <param name="storageRules">Storage rules to set.</param>
        private static void SetStorageRules(string storageRules)
        {
            switch (storageRules)
            {
                case "file":
                    FileStream fileStream = new FileStream(FILENAME, FileMode.Create);
                    fileCabinetService = new FileCabinetFilesystemService(fileStream, validator);

                    break;
                default:
                    fileCabinetService = new FileCabinetMemoryService(validator);
                    break;
            }
        }

        /// <summary>
        /// Shows the number of records to the user.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        private static void Stat(string parameters)
        {
            var recordsCount = fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        /// <summary>
        /// Creates a new record.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        private static void Create(string parameters)
        {
            int id = fileCabinetService.CreateRecord(CheckRecordInput());
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
                if (id < 1 || !fileCabinetService.GetIds().Contains(id))
                {
                    Console.WriteLine($"#{id} record is not found.");
                    return;
                }
                else
                {
                    fileCabinetService.EditRecord(CheckRecordInput(id));
                    Console.WriteLine($"Record #{id} is updated.");
                }
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
                        foundRecords = fileCabinetService.FindByFirstName(args[1]);
                        ShowRecords(foundRecords);
                        break;

                    case "lastname":
                        foundRecords = fileCabinetService.FindByLastName(args[1]);
                        ShowRecords(foundRecords);
                        break;

                    case "dateofbirth":
                        foundRecords = fileCabinetService.FindByDateOfBirth(args[1]);
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
                IFileCabinetServiceSnapshot snapshot = fileCabinetService.MakeSnapshot();
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
                        importedRecords = fileCabinetService.Restore(snapshot);
                        break;

                    case "xml":
                        records = snapshot.LoadFromXml(reader);
                        snapshot = new FileCabinetServiceSnapshot(records);
                        importedRecords = fileCabinetService.Restore(snapshot);
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
            ShowRecords(fileCabinetService.GetRecords());
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

                var validationResult = validator(fileCabinetService.GetValidator(), field, value);
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
                var index = Array.FindIndex(HelpMessages, 0, HelpMessages.Length, i => string.Equals(i[Program.COMMANDHELPINDEX], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(HelpMessages[index][Program.EXPLANATIONHELPINDEX]);
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
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.COMMANDHELPINDEX], helpMessage[Program.DESCRIPTIONHELPINDEX]);
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
            fileCabinetService.Close();
            isRunning = false;
        }
    }
}