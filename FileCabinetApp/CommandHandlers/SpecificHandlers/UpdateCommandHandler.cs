using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using FileCabinetApp.Helpers;
using FileCabinetApp.Printers;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the user's 'update' command.
    /// </summary>
    public class UpdateCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Service to create record in.</param>
        public UpdateCommandHandler(IFileCabinetService fileCabinetService)
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

            if (request.Command.ToLower() == "update")
            {
                this.Update(request.Parameters);
                return true;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private void Update(string parameters)
        {
            int indexOfSet = parameters.IndexOf("set");
            if (indexOfSet == -1)
            {
                return;
            }

            // make safe / +4
            parameters = parameters.Substring(indexOfSet + 3);

            int numOfProps = typeof(FileCabinetRecord).GetProperties().Length;
            string[] sets = new string[numOfProps];

            // propertyName and its index in the dictionary
            Dictionary<string, int> propIndex = new Dictionary<string, int>();

            var properties = typeof(FileCabinetRecord).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                // set the indexes
                propIndex.Add(properties[i].Name.ToLower(), i);
            }

            string[] set = this.MakeSet(parameters);
            if (set == null)
            {
                Console.WriteLine("Incorrect to-set conditions.");
                return;
            }

            List<FileCabinetRecord> foundRecords = this.WhereParser(parameters.Substring(parameters.IndexOf("where") + 6).Split());
            List<FileCabinetRecord> copyOfRecords = new List<FileCabinetRecord>();

            if (foundRecords == null)
            {
                Console.WriteLine("No records found and changed.");
                return;
            }

            foreach (var record in foundRecords)
            {
                copyOfRecords.Add((FileCabinetRecord)record.Clone());
            }

            // for each property
            for (int i = 0; i < set.Length; i++)
            {
                if (set[i] != null)
                {
                    // for each record in foundRecord
                    for (int j = 0; j < foundRecords.Count; j++)
                    {
                        PropertyInfo prop = typeof(FileCabinetRecord).GetProperty(propIndex.FirstOrDefault(x => x.Value == i).Key, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                        // get the type of property
                        Type type = prop.PropertyType;

                        dynamic value = null;

                        // convert value to the type of property
                        if (type.Name == "DateTime")
                        {
                            try
                            {
                                value = DateTime.ParseExact(set[i], "yyyy-MMM-d", CultureInfo.InvariantCulture);
                            }
                            catch (FormatException)
                            {
                            }
                        }
                        else
                        {
                            if (type.Name == "String")
                            {
                                value = set[i];
                            }
                            else
                            {
                                value = Convert.ChangeType(set[i], type, CultureInfo.InvariantCulture);
                            }
                        }

                        if (ReferenceEquals(null, value))
                        {
                            Console.WriteLine("Convertion of " + prop.Name + " failed.");
                            return;
                        }

                        prop.SetValue(copyOfRecords[j], value);
                    }
                }
            }

            foreach (var record in copyOfRecords)
            {
                this.service.AddRecord(record);
            }

             DefaultRecordPrinter printer = new DefaultRecordPrinter();
             printer.Print(this.service.GetRecords());
        }
    }
}
