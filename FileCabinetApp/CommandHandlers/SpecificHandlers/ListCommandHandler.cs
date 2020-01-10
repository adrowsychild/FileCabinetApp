using System;
using System.Collections.Generic;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the user's 'list' command.
    /// </summary>
    public class ListCommandHandler : ServiceCommandHandlerBase
    {
        private readonly IRecordPrinter printer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ListCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Service to show records from.</param>
        /// <param name="printer">Printer for records.</param>
        public ListCommandHandler(IFileCabinetService fileCabinetService, IRecordPrinter printer)
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

            if (request.Command.ToLower() == "list")
            {
                this.List();
                return true;
            }
            else
            {
                return base.Handle(request);
            }
        }

        /// <summary>
        /// Shows all the records to the user.
        /// </summary>
        private void List()
        {
            this.printer.Print(this.service.GetRecords());
        }
    }
}
