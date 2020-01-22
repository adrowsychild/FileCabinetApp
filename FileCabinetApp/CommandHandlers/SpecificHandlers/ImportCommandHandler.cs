using System;
using System.Collections.Generic;
using System.IO;
using FileCabinetApp.Interfaces;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the user's 'import' command.
    /// </summary>
    public class ImportCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImportCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Service to import records to.</param>
        public ImportCommandHandler(IFileCabinetService fileCabinetService)
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

            if (request.Command.ToLower() == "import")
            {
                this.Import(request.Parameters);
                return true;
            }
            else
            {
                return base.Handle(request);
            }
        }

        /// <summary>
        /// Imports the records from csv or xml file.
        /// </summary>
        /// <param name="parameters">Format to write in and path to write to.</param>
        private void Import(string parameters)
        {
            string[] args = parameters.Split();
            if (args == null || args.Length < 2)
            {
                Console.WriteLine("Incorrect parameters.");
                return;
            }

            string format = args[0].ToLower();
            string path = args[1];

            if (!File.Exists(path))
            {
                Console.WriteLine("Import error: " + path + " does not exist");
                return;
            }

            using (StreamReader reader = new StreamReader(path))
            {
                int importedRecords = 0;
                IList<FileCabinetRecord> records;
                IFileCabinetServiceSnapshot snapshot = new FileCabinetServiceSnapshot();
                switch (format)
                {
                    case "csv":
                        records = snapshot.LoadFromCsv(reader);
                        snapshot = new FileCabinetServiceSnapshot(records);
                        importedRecords = this.Service.Restore(snapshot);
                        break;

                    case "xml":
                        records = snapshot.LoadFromXml(reader);
                        snapshot = new FileCabinetServiceSnapshot(records);
                        importedRecords = this.Service.Restore(snapshot);
                        break;

                    default:
                        Console.WriteLine("Incorrect format: can be xml or csv.");
                        return;
                }

                Console.WriteLine(importedRecords + " records were imported from " + path + ".");
            }
        }
    }
}
