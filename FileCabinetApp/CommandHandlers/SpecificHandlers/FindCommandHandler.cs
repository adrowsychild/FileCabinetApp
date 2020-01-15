using System;
using System.Collections.Generic;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the user's 'find' command.
    /// </summary>
    public class FindCommandHandler : ServiceCommandHandlerBase
    {
        private readonly IRecordPrinter printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="FindCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Service to find record in.</param>
        /// <param name="printer">Printer for records.</param>
        public FindCommandHandler(IFileCabinetService fileCabinetService, IRecordPrinter printer)
        {
            this.service = fileCabinetService;
            this.printer = printer;
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

            IEnumerable<FileCabinetRecord> foundRecords;

            try
            {
                switch (args[0].ToLower())
                {
                    case "firstname":
                        foundRecords = this.service.FindByFirstName(args[1]);
                        this.printer.Print(foundRecords);
                        break;

                    case "lastname":
                        foundRecords = this.service.FindByLastName(args[1]);
                        this.printer.Print(foundRecords);
                        break;

                    case "dateofbirth":
                        foundRecords = this.service.FindByDateOfBirth(args[1]);
                        this.printer.Print(foundRecords);
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
