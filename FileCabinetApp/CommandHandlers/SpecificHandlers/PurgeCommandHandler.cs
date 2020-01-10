﻿using System;
using System.Collections.Generic;
using System.Reflection;
using static FileCabinetApp.Program;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the user's 'purge' command.
    /// </summary>
    public class PurgeCommandHandler : CommandHandlerBase
    {
        private readonly IFileCabinetService fileCabinetService;

        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Service to purge.</param>
        public PurgeCommandHandler(IFileCabinetService fileCabinetService)
        {
            this.fileCabinetService = fileCabinetService;
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

            if (request.Command.ToLower() == "purge")
            {
                this.Purge();
                return true;
            }
            else
            {
                return base.Handle(request);
            }
        }

        /// <summary>
        /// Purges the list of records, if fileSystemService.
        /// </summary>
        private void Purge()
        {
            if (this.fileCabinetService.GetType() == typeof(FileCabinetFilesystemService))
            {
                int initialNumOfRecords = this.fileCabinetService.GetStat();

                MethodInfo method = typeof(FileCabinetFilesystemService).GetMethod("Purge");
                int recordsPurged = (int)method.Invoke(this.fileCabinetService, null);
                Console.WriteLine("Data file processing is completed: " + recordsPurged + " of " + initialNumOfRecords + " records were purged.");
            }
        }
    }
}
