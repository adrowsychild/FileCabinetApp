using System;
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
                Purge();
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
        private static void Purge()
        {
            if (FileCabinetService.GetType() == typeof(FileCabinetFilesystemService))
            {
                int initialNumOfRecords = FileCabinetService.GetStat();

                MethodInfo method = typeof(FileCabinetFilesystemService).GetMethod("Purge");
                int recordsPurged = (int)method.Invoke(FileCabinetService, null);
                Console.WriteLine("Data file processing is completed: " + recordsPurged + " of " + initialNumOfRecords + " records were purged.");
            }
        }
    }
}
