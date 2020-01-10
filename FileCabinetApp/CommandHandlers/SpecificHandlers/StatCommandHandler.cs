using System;
using System.Collections.Generic;
using static FileCabinetApp.Program;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the user's 'stat' command.
    /// </summary>
    public class StatCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StatCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Service to get stats from.</param>
        public StatCommandHandler(IFileCabinetService fileCabinetService)
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

            if (request.Command.ToLower() == "stat")
            {
                this.Stat();
                return true;
            }
            else
            {
                return base.Handle(request);
            }
        }

        /// <summary>
        /// Shows the number of records to the user.
        /// </summary>
        private void Stat()
        {
            var recordsCount = this.service.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
            Console.WriteLine($"{this.service.GetDeleted()} records are ready to be purged.");
        }
    }
}
