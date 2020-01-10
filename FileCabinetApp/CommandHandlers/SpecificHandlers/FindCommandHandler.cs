using System;
using System.Collections.Generic;
using FileCabinetApp.Interfaces;
using static FileCabinetApp.Program;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the user's 'find' command.
    /// </summary>
    public class FindCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Service to find record in.</param>
        public FindCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.service = fileCabinetService;
        }

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
                this.Find(request.Parameters);
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
        private void Find(string parameters)
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
                        foundRecords = this.service.FindByFirstName(args[1]);
                        ShowRecords(foundRecords);
                        break;

                    case "lastname":
                        foundRecords = this.service.FindByLastName(args[1]);
                        ShowRecords(foundRecords);
                        break;

                    case "dateofbirth":
                        foundRecords = this.service.FindByDateOfBirth(args[1]);
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
