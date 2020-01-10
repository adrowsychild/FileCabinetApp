using System;
using System.Collections.Generic;
using static FileCabinetApp.Program;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the user's 'edit' command.
    /// </summary>
    public class EditCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Service to edit record in.</param>
        public EditCommandHandler(IFileCabinetService fileCabinetService)
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

            if (request.Command.ToLower() == "edit")
            {
                this.Edit(request.Parameters);
                return true;
            }
            else
            {
                return base.Handle(request);
            }
        }

        /// <summary>
        /// Edits the existing record.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        private void Edit(string parameters)
        {
            if (int.TryParse(parameters, out int id))
            {
                if (id < 1 || !this.service.GetIds().Contains(id))
                {
                    Console.WriteLine($"#{id} record is not found.");
                    return;
                }
                else
                {
                    this.service.EditRecord(this.CheckRecordInput(id));
                    Console.WriteLine($"Record #{id} is updated.");
                }
            }
        }
    }
}
