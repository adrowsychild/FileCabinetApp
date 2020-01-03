using System;
using System.Collections.Generic;
using System.Globalization;

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
        };

        private static bool isRunning = true;

        private static FileCabinetService fileCabinetService;

        private delegate void Settings(string args);

        /// <summary>
        /// Method for handling user's commands.
        /// </summary>
        /// <param name="args">Additional rule-changing arguments.</param>
        public static void Main(string[] args)
        {
            if (args != null && args.Length > 0)
            {
                SettingsParser(args);
            }
            else
            {
                SetDefaultSettings();
            }

            Console.WriteLine(Program.INTRO);

            int serviceIndex = fileCabinetService.GetType().ToString().IndexOf("Service", StringComparison.InvariantCulture);
            string validationRules = fileCabinetService.GetType().ToString()[26..serviceIndex].ToLower();

            Console.WriteLine("Using " + validationRules + " validation rules.");

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
        private static void SettingsParser(string[] args)
        {
            SetDefaultSettings();

            string operation = string.Empty;
            string parameter = string.Empty;
            Settings makeChanges = null;

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
                }
            }

            if (!string.IsNullOrEmpty(operation) && !string.IsNullOrEmpty(parameter) && makeChanges != null)
            {
                makeChanges.Invoke(parameter.ToLower());
            }
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
            fileCabinetService = validationRules switch
            {
                "custom" => new FileCabinetCustomService(),
                _ => new FileCabinetDefaultService(),
            };
        }

        private static void Stat(string parameters)
        {
            var recordsCount = fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            CheckRecordInput("create");
            Console.WriteLine($"Record # {fileCabinetService.GetStat()} is created.");
        }

        private static void Edit(string parameters)
        {
            if (int.TryParse(parameters, out int id))
            {
                if (id < 1 || id > fileCabinetService.GetStat())
                {
                    Console.WriteLine($"#{id} record is not found.");
                    return;
                }

                id--;
                CheckRecordInput("edit", id);
                Console.WriteLine($"Record #{++id} is updated.");
            }
        }

        private static void Find(string parameters)
        {
            string[] args = parameters.Split();
            if (args.Length > 2)
            {
                return;
            }

            FileCabinetRecord[] foundRecords;

            try
            {
                switch (args[0].ToLower())
                {
                    case "firstname":
                        foundRecords = fileCabinetService.FindByFirstName(args[1]);

                        foreach (var record in foundRecords)
                        {
                            ShowRecord(record);
                        }

                        break;
                    case "lastname":
                        foundRecords = fileCabinetService.FindByLastName(args[1]);

                        foreach (var record in foundRecords)
                        {
                            ShowRecord(record);
                        }

                        break;
                    case "dateofbirth":
                        foundRecords = fileCabinetService.FindByDateOfBirth(args[1]);

                        foreach (var record in foundRecords)
                        {
                            ShowRecord(record);
                        }

                        break;
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static void List(string parameters)
        {
            FileCabinetRecord[] tempList = fileCabinetService.GetRecords();

            foreach (var record in tempList)
            {
                ShowRecord(record);
            }
        }

        private static void ShowRecord(FileCabinetRecord record)
        {
            Console.WriteLine($"#{record.Id}, {record.FirstName}, {record.LastName}, {record.DateOfBirth.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture)}, favourite number: {record.FavouriteNumber}, favourite character: {record.FavouriteCharacter}, favourite game: {record.FavouriteGame}, donations: {record.Donations}");
        }

        private static void CheckRecordInput(string action, int id = 0)
        {
        tryinput:
            Console.WriteLine("First Name: ");
            string tempFirstName = Console.ReadLine();

            Console.WriteLine("Last Name: ");
            string tempLastName = Console.ReadLine();

            Console.WriteLine("Date of Birth: ");
            DateTime.TryParseExact(Console.ReadLine(), "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime tempDateOfBirth);

            Console.WriteLine("Favourite number: ");
            short.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture, out short tmpFavouriteNumber);

            Console.WriteLine("Favourite character: ");
            char tmpFavouriteCharacter = Console.ReadLine()[0];

            Console.WriteLine("Favourite game: ");
            string tmpFavouriteGame = Console.ReadLine();

            Console.WriteLine("Donations: ");
            decimal.TryParse(Console.ReadLine(), NumberStyles.Any, CultureInfo.InvariantCulture, out decimal tmpDonations);

            try
            {
                switch (action)
                {
                    case "create":
                        fileCabinetService.CreateRecord(new FileCabinetRecord(id, tempFirstName, tempLastName, tempDateOfBirth, tmpFavouriteNumber, tmpFavouriteCharacter, tmpFavouriteGame, tmpDonations));
                        break;
                    case "edit":
                        fileCabinetService.EditRecord(new FileCabinetRecord(id, tempFirstName, tempLastName, tempDateOfBirth, tmpFavouriteNumber, tmpFavouriteCharacter, tmpFavouriteGame, tmpDonations));
                        break;
                }
            }
            catch (ArgumentException ex)
            {
                if (ex.Message == "id is invalid.")
                {
                    Console.WriteLine($"#{id} record is not found.");
                    return;
                }

                Console.WriteLine($"{ex.Message} Please, try again.");
                Console.WriteLine("Hint:");

                string help = "First Name: from 2 to 60 symbols\n" +
                              "Last Name:  from 2 to 60 symbols\n" +
                              "Data of Birth: Month/Day/Year since 01-Jan-1950\n" +
                              "Favourite number: integer, not negative\n" +
                              "Favourite character: latin alphabet\n" +
                              "Donations: not negative number\n";
                Console.WriteLine(help);
                goto tryinput;
            }
        }

        private static void PrintMissedCommandInfo(string command)
        {
            Console.WriteLine($"There is no '{command}' command.");
            Console.WriteLine();
        }

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

        private static void Exit(string parameters)
        {
            Console.WriteLine("Exiting an application...");
            isRunning = false;
        }
    }
}