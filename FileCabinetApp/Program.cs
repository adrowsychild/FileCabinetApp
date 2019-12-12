using System;
using System.Globalization;

namespace FileCabinetApp
{
    public static class Program
    {
        private const string DeveloperName = "Dyl Aliaksandra";
        private const string HintMessage = "Enter your command, or enter 'help' to get help.";
        private const int CommandHelpIndex = 0;
        private const int DescriptionHelpIndex = 1;
        private const int ExplanationHelpIndex = 2;

        private static FileCabinetService fileCabinetService = new FileCabinetService();

        private static bool isRunning = true;

        private static Tuple<string, Action<string>>[] commands = new Tuple<string, Action<string>>[]
        {
            new Tuple<string, Action<string>>("help", PrintHelp),
            new Tuple<string, Action<string>>("exit", Exit),
            new Tuple<string, Action<string>>("stat", Stat),
            new Tuple<string, Action<string>>("create", Create),
            new Tuple<string, Action<string>>("list", List),
            new Tuple<string, Action<string>>("edit", Edit),
            new Tuple<string, Action<string>>("find", Find),
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints the number of records", "The 'stat' command prints the number of records." },
            new string[] { "create", "creates a new record", "The 'create' command creates a new record." },
            new string[] { "edit", "edits the existing record", "The 'edit' command edits the existing record." },
            new string[] { "find", "finds records by given criteria", "The 'find' command finds records by given criteria" },
            new string[] { "list", "prints the records", "The 'list' prints the records." },
        };

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            CheckRecordInput("create", -1);
            Console.WriteLine("Record #" + fileCabinetService.GetStat() + " is created.");
        }

        private static void Edit(string parameters)
        {
            if (int.TryParse(parameters, out int id))
            {
                if (id < 1 || id > fileCabinetService.GetStat())
                {
                    Console.WriteLine("#" + id + " record is not found.");
                    return;
                }

                id--;
                CheckRecordInput("edit", id);
                Console.WriteLine("Record #" + ++id + " is updated.");
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
            Console.WriteLine("#" + record.Id + ", " + record.FirstName + ", " + record.LastName + ", " + record.DateOfBirth.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture) + ", favourite number: " + record.FavouriteNumber + ", favourite character: " + record.FavouriteCharacter + ", favourite game: " + record.FavouriteGame + ", donations: " + record.Donations);
        }

        private static void CheckRecordInput(string action, int id)
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
                        fileCabinetService.CreateRecord(tempFirstName, tempLastName, tempDateOfBirth, tmpFavouriteNumber, tmpFavouriteCharacter, tmpFavouriteGame, tmpDonations);
                        break;
                    case "edit":
                        fileCabinetService.EditRecord(id, tempFirstName, tempLastName, tempDateOfBirth, tmpFavouriteNumber, tmpFavouriteCharacter, tmpFavouriteGame, tmpDonations);
                        break;
                }
            }
            catch (ArgumentException ex)
            {
                if (ex.Message == "id is invalid.")
                {
                    Console.WriteLine("#" + id + " record is not found.");
                    return;
                }

                Console.WriteLine(ex.Message + " Please, try again.");
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

        public static void Main(string[] args)
        {
            Console.WriteLine($"File Cabinet Application, developed by {Program.DeveloperName}");
            Console.WriteLine(Program.HintMessage);
            Console.WriteLine();

            do
            {
                Console.Write("> ");
                var inputs = Console.ReadLine().Split(' ', 2);
                const int commandIndex = 0;
                var command = inputs[commandIndex];

                if (string.IsNullOrEmpty(command))
                {
                    Console.WriteLine(Program.HintMessage);
                    continue;
                }

                var index = Array.FindIndex(commands, 0, commands.Length, i => i.Item1.Equals(command, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    const int parametersIndex = 1;
                    var parameters = inputs.Length > 1 ? inputs[parametersIndex] : string.Empty;
                    commands[index].Item2(parameters);
                }
                else
                {
                    PrintMissedCommandInfo(command);
                }
            }
            while (isRunning);
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
                var index = Array.FindIndex(helpMessages, 0, helpMessages.Length, i => string.Equals(i[Program.CommandHelpIndex], parameters, StringComparison.InvariantCultureIgnoreCase));
                if (index >= 0)
                {
                    Console.WriteLine(helpMessages[index][Program.ExplanationHelpIndex]);
                }
                else
                {
                    Console.WriteLine($"There is no explanation for '{parameters}' command.");
                }
            }
            else
            {
                Console.WriteLine("Available commands:");

                foreach (var helpMessage in helpMessages)
                {
                    Console.WriteLine("\t{0}\t- {1}", helpMessage[Program.CommandHelpIndex], helpMessage[Program.DescriptionHelpIndex]);
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