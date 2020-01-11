using System;
using System.Reflection;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the user's 'purge' command.
    /// </summary>
    public class PurgeCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PurgeCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Service to purge.</param>
        public PurgeCommandHandler(IFileCabinetService fileCabinetService)
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
            if (this.service.GetType() == typeof(FileCabinetFilesystemService))
            {
                int initialNumOfRecords = this.service.GetStat();

                MethodInfo method = typeof(FileCabinetFilesystemService).GetMethod("Purge");
                int recordsPurged = (int)method.Invoke(this.service, null);
                Console.WriteLine("Data file processing is completed: " + recordsPurged + " of " + initialNumOfRecords + " records were purged.");
            }
        }
    }
}
