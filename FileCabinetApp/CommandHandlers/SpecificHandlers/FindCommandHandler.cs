using System;
using System.Collections.Generic;
using FileCabinetApp.Interfaces;
using static FileCabinetApp.Program;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the user's 'find' command.
    /// </summary>
    public class FindCommandHandler : CommandHandlerBase
    {
        /// <summary>
        /// Handles the request.
        /// </summary>
        /// <param name="request">Request to handle.</param>
        /// <returns>The object back, or null otherwise.</returns>
        public override object Handle(AppCommandRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException($"Request is null.");
            }

            if (request.Command.ToLower() == "find")
            {
                Find(request.Parameters);
                return true;
            }
            else
            {
                return base.Handle(request);
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
    }
}
