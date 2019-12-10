﻿using System;
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
        };

        private static string[][] helpMessages = new string[][]
        {
            new string[] { "help", "prints the help screen", "The 'help' command prints the help screen." },
            new string[] { "exit", "exits the application", "The 'exit' command exits the application." },
            new string[] { "stat", "prints the number of records.", "The 'stat' command prints the number of records." },
            new string[] { "create", "creates a new record.", "The 'create' command creates a new record." },
            new string[] { "list", "prints the records.", "The 'list' prints the records." },
        };

        private static void Stat(string parameters)
        {
            var recordsCount = Program.fileCabinetService.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
        }

        private static void Create(string parameters)
        {
            Console.WriteLine("First Name: ");
            string tempFirstName = Console.ReadLine();
            Console.WriteLine("Last Name: ");
            string tempLastName = Console.ReadLine();
            Console.WriteLine("Date of Birth: ");
            DateTime tempDateOfBirth = DateTime.ParseExact(Console.ReadLine(), "M/d/yyyy", CultureInfo.InvariantCulture);
            Console.WriteLine("Favourite number: ");
            short tmpFavouriteNumber = short.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);
            Console.WriteLine("Favourite character: ");
            char tmpFavouriteCharacter = Console.ReadLine()[0];
            Console.WriteLine("Favourite game: ");
            string tmpFavouriteGame = Console.ReadLine();
            Console.WriteLine("Donations: ");
            decimal tmpDonations = decimal.Parse(Console.ReadLine(), CultureInfo.InvariantCulture);
            fileCabinetService.CreateRecord(tempFirstName, tempLastName, tempDateOfBirth, tmpFavouriteNumber, tmpFavouriteCharacter, tmpFavouriteGame, tmpDonations);
            Console.WriteLine("Record #" + fileCabinetService.GetStat() + " is created.");
        }

        private static void List(string parameters)
        {
            FileCabinetRecord[] tempList = fileCabinetService.GetRecords();
            foreach (var record in tempList)
            {
                Console.WriteLine("#" + record.Id + ", " + record.FirstName + ", " + record.LastName + ", " + record.DateOfBirth.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture) + ", favourite number: " + record.FavouriteNumber + ", favourite character: " + record.FavouriteCharacter + ", favourite game: " + record.FavouriteGame + ", donations: " + record.Donations);
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