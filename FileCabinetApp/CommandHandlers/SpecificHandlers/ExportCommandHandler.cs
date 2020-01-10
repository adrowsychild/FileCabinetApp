using System;
using System.Collections.Generic;
using System.IO;
using FileCabinetApp.Interfaces;
using static FileCabinetApp.Program;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the user's 'export' command.
    /// </summary>
    public class ExportCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExportCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Service to export records from.</param>
        public ExportCommandHandler(IFileCabinetService fileCabinetService)
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

            if (request.Command.ToLower() == "export")
            {
                this.Export(request.Parameters);
                return true;
            }
            else
            {
                return base.Handle(request);
            }
        }

        /// <summary>
        /// Exports the records to csv or xml format.
        /// </summary>
        /// <param name="parameters">Format to write in and path to write to.</param>
        private void Export(string parameters)
        {
            string[] args = parameters.Split();
            if (args == null || args.Length < 2)
            {
                Console.WriteLine("Incorrect parameters.");
                return;
            }

            string format = args[0].ToLower();
            string path = args[1];

            if (File.Exists(path))
            {
                Console.WriteLine("File is exist - rewrite " + path + "? [Y/n]");
                if (Console.ReadLine().ToLower() != "y")
                {
                    return;
                }
            }

            using (StreamWriter writer = new StreamWriter(path))
            {
                IFileCabinetServiceSnapshot snapshot = this.service.MakeSnapshot();
                bool isSucceed = false;
                switch (format)
                {
                    case "csv":
                        isSucceed = snapshot.SaveToCsv(writer);
                        break;

                    case "xml":
                        isSucceed = snapshot.SaveToXml(writer);
                        break;

                    default:
                        Console.WriteLine("Incorrect format: can be xml or csv.");
                        return;
                }

                if (!isSucceed)
                {
                    Console.WriteLine("Export failed: can't open file " + path);
                }
                else
                {
                    Console.WriteLine("All records are exported to file " + path);
                }
            }
        }
    }
}
