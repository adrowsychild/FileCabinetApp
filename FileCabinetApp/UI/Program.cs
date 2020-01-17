using System;
using System.Collections.Generic;
using System.IO;
using FileCabinetApp.CommandHandlers;
using FileCabinetApp.Extensions.ValidatorExtensions;
using FileCabinetApp.Printers;
using FileCabinetApp.Validators;

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

        private static bool isRunning = true;

        private static IFileCabinetService fileCabinetService;

        private static IRecordValidator validator;

        private static string validatorType;

        private static string serviceType;

        private static bool stopwatchAdded;
        private static bool loggerAdded;

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

                    if (tempArgs[0] == "--use-stopwatch")
                    {
                        stopwatchAdded = true;
                        i++;
                        continue;
                    }
                    else if (tempArgs[0] == "--use-logger")
                    {
                        loggerAdded = true;
                        i++;
                        continue;
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

            Console.WriteLine("Using " + validatorType + " validation rules.");
            Console.WriteLine("Using " + serviceType + " service type.");

            Console.WriteLine(Program.HINTMESSAGE);
            Console.WriteLine();

            StreamWriter logWriter = null;

            if (loggerAdded)
            {
                logWriter = new StreamWriter("logs.txt", true, System.Text.Encoding.Default);
                fileCabinetService = new ServiceLogger(fileCabinetService, logWriter);
            }

            if (stopwatchAdded)
            {
                fileCabinetService = new ServiceMeter(fileCabinetService);
            }

            var commandHandler = CreateCommandHandlers();

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
            while (isRunning);

            if (loggerAdded == true)
            {
                logWriter.Close();
            }
        }

        private static void ChangeServiceState(bool toSet)
        {
            isRunning = toSet;
        }

        /// <summary>
        /// Creates the chain of command handlers.
        /// </summary>
        /// <returns>The first handler of the chain.</returns>
        private static ICommandHandler CreateCommandHandlers()
        {
            var recordPrinter = new DefaultRecordPrinter();
            var createHandler = new CreateCommandHandler(fileCabinetService);
            var editHandler = new EditCommandHandler(fileCabinetService);
            var insertHandler = new InsertCommandHandler(fileCabinetService);
            var deleteHandler = new DeleteCommandHandler(fileCabinetService);
            var removeHandler = new RemoveCommandHandler(fileCabinetService);
            var purgeHandler = new PurgeCommandHandler(fileCabinetService);
            var importHandler = new ImportCommandHandler(fileCabinetService);
            var exportHandler = new ExportCommandHandler(fileCabinetService);
            var findHandler = new FindCommandHandler(fileCabinetService, recordPrinter);
            var listHandler = new ListCommandHandler(fileCabinetService, recordPrinter);
            var statHandler = new StatCommandHandler(fileCabinetService);
            var helpHandler = new HelpCommandHandler();
            var exitHandler = new ExitCommandHandler(ChangeServiceState);

            createHandler.SetNext(editHandler).SetNext(insertHandler).SetNext(deleteHandler).SetNext(removeHandler).SetNext(purgeHandler).SetNext(importHandler).SetNext(exportHandler).SetNext(findHandler).SetNext(listHandler).SetNext(statHandler).SetNext(helpHandler).SetNext(exitHandler);

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
            switch (validationRules)
            {
                case "custom":
                    validator = ValidatorExtension.CreateСustom(new ValidatorBuilder());
                    validatorType = "custom";
                    break;
                default:
                    validator = ValidatorExtension.CreateDefault(new ValidatorBuilder());
                    validatorType = "default";
                    break;
            }
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
                    serviceType = "file";

                    break;
                default:
                    fileCabinetService = new FileCabinetMemoryService(validator);
                    serviceType = "memory";
                    break;
            }
        }

        private static void AddStopwatch(string args)
        {
            stopwatchAdded = true;
        }
    }
}