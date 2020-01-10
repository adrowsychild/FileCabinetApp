using System;
using System.Collections.Generic;
using System.IO;
using FileCabinetApp.CommandHandlers;

namespace FileCabinetApp
{
    /// <summary>
    /// Class for handling user's commands.
    /// </summary>
    public static class Program
    {
        /// <summary>
        /// Developer's name.
        /// </summary>
        public const string DEVELOPERNAME = "Dyl Aliaksandra";

        /// <summary>
        /// Hint message to show the user.
        /// </summary>
        public const string HINTMESSAGE = "Enter your command, or enter 'help' to get help.";

        /// <summary>
        /// Intro message of the program.
        /// </summary>
        public const string INTRO = "File Cabinet Application, developed by " + DEVELOPERNAME;

        /// <summary>
        /// Filename of the data storage.
        /// </summary>
        public const string FILENAME = "cabinet-records.db";

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

        private static IFileCabinetService fileCabinetService;

        private static IRecordValidator validator;

        private delegate void Settings(string args);

        /// <summary>
        /// Gets or sets a value indicating whether the program is running or not.
        /// </summary>
        /// <value>Whether the program is running or not.</value>
        public static bool IsRunning { get; set; }

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

            var commandHandler = CreateCommandHandlers();
            IsRunning = true;

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

                const int parametersIndex = 1;
                var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                commandHandler.Handle(new AppCommandRequest(command, parameters));
            }
            while (IsRunning);
        }

        /// <summary>
        /// Creates the chain of command handlers.
        /// </summary>
        /// <returns>The first handler of the chain.</returns>
        private static ICommandHandler CreateCommandHandlers()
        {
            var createHandler = new CreateCommandHandler(fileCabinetService);
            var editHandler = new EditCommandHandler(fileCabinetService);
            var removeHandler = new RemoveCommandHandler(fileCabinetService);
            var purgeHandler = new PurgeCommandHandler(fileCabinetService);
            var importHandler = new ImportCommandHandler(fileCabinetService);
            var exportHandler = new ExportCommandHandler(fileCabinetService);
            var findHandler = new FindCommandHandler(fileCabinetService);
            var listHandler = new ListCommandHandler(fileCabinetService);
            var statHandler = new StatCommandHandler(fileCabinetService);
            var helpHandler = new HelpCommandHandler();

            createHandler.SetNext(editHandler).SetNext(removeHandler).SetNext(purgeHandler).SetNext(importHandler).SetNext(exportHandler).SetNext(findHandler).SetNext(listHandler).SetNext(statHandler).SetNext(helpHandler);

            return createHandler;
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
    }
}