﻿using System;
using System.Collections.Generic;

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
            this.Service = fileCabinetService;
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
            var recordsCount = this.Service.GetStat();
            Console.WriteLine($"{recordsCount} record(s).");
            Console.WriteLine($"{this.Service.GetDeleted()} records are ready to be purged.");
        }
    }
}
