using System;
using System.Collections.Generic;
using System.Reflection;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the user's 'delete' command.
    /// </summary>
    public class DeleteCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Service to create record in.</param>
        public DeleteCommandHandler(IFileCabinetService fileCabinetService)
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

            if (request.Command.ToLower() == "delete")
            {
                this.Delete(request.Parameters);
                return true;
            }
            else
            {
                return base.Handle(request);
            }
        }

        /// <summary>
        /// Deletes the records by the given criterion.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        private void Delete(string parameters)
        {
            string[] args = parameters.Split(' ');
            if (string.IsNullOrEmpty(parameters) || args.Length < 2 || args[0].ToLower() != "where")
            {
                return;
            }

            string property;
            string value;
            int equalsIndex = -1;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Contains('=', StringComparison.InvariantCulture))
                {
                    equalsIndex = i;
                }
            }

            if (equalsIndex == -1 || equalsIndex == 0 || equalsIndex >= 3)
            {
                return;
            }

            if (equalsIndex == 2)
            {
                // where id = '1'
                if (args[2] == "=")
                {
                    value = args[3];
                }
                else
                {
                    // where id ='1'
                    if (args[2].StartsWith('='))
                    {
                        // or (2)?
                        value = args[2].Substring(1);
                    }
                    else
                    {
                        return;
                    }
                }

                property = args[1];
            }
            else
            {
                if (args[1].EndsWith('='))
                {
                    property = args[1].Substring(0, args[1].Length - 1);
                    value = args[2];
                }
                else
                {
                    string[] subArgs = args[1].Split('=');
                    value = subArgs[1];
                    property = subArgs[0];
                }
            }

            if (!value.Contains('\'', StringComparison.InvariantCulture) || (value.IndexOf('\'', StringComparison.InvariantCulture) == value.LastIndexOf('\'')))
            {
                return;
            }
            else
            {
                value = value[(value.IndexOf('\'', StringComparison.InvariantCulture) + 1) ..value.LastIndexOf('\'')];
            }

            MethodInfo findMethod = this.Service.GetType().GetMethod("findby" + property, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (findMethod == null)
            {
                // incorrect property
                return;
            }

            List<FileCabinetRecord> foundRecords = (List<FileCabinetRecord>)findMethod.Invoke(this.Service, new object[] { value });

            if (foundRecords == null)
            {
                Console.WriteLine("No such records found.");
                return;
            }

            List<int> deletedRecords = new List<int>();

            foreach (var record in foundRecords)
            {
                deletedRecords.Add(record.Id);
            }

            foreach (var id in deletedRecords)
            {
                this.Service.RemoveRecord(id);
            }

            Console.Write("Records ");

            for (int i = 0; i < deletedRecords.Count; i++)
            {
                Console.Write("#" + deletedRecords[i]);

                if (i != deletedRecords.Count - 1)
                {
                    Console.Write(", ");
                }
            }

            if (deletedRecords.Count == 1)
            {
                Console.WriteLine(" is deleted.");
            }
            else
            {
                Console.WriteLine(" are deleted.");
            }
        }
    }
}
