using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text.RegularExpressions;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the user's 'insert' command.
    /// </summary>
    public class InsertCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InsertCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Service to get stats from.</param>
        public InsertCommandHandler(IFileCabinetService fileCabinetService)
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

            if (request.Command.ToLower() == "insert")
            {
                this.Insert(request.Parameters);
                return true;
            }
            else
            {
                return base.Handle(request);
            }
        }

        private void Insert(string parameters)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                return;
            }

            int valueIndex = -1;
            string[] args = parameters.Split(new char[] { ',', '(', ')' });

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].ToLower().Contains("values", StringComparison.InvariantCulture) || args[i].ToLower().Contains("value", StringComparison.InvariantCulture))
                {
                    valueIndex = i;
                    break;
                }
            }

            if (valueIndex == -1)
            {
                return;
            }

            string[] properties = new string[valueIndex];
            string[] values = new string[args.Length - (valueIndex + 1)];
            int propEmptyValues = 0;
            int valEmptyValues = 0;

            for (int i = 0; i < valueIndex; i++)
            {
                if (string.IsNullOrEmpty(args[i]))
                {
                    propEmptyValues++;
                    continue;
                }

                properties[i - propEmptyValues] = args[i];
                properties[i - propEmptyValues] = Regex.Replace(properties[i - propEmptyValues], @"\s+", string.Empty);
            }

            for (int i = valueIndex + 1; i < args.Length; i++)
            {
                if (string.IsNullOrEmpty(args[i]))
                {
                    valEmptyValues++;
                    continue;
                }

                if (args[i].IndexOf('\'', StringComparison.InvariantCulture) == args[i].LastIndexOf('\''))
                {
                    return;
                }

                // insert without ''
                values[i - (valueIndex + 1) + valEmptyValues] = args[i][(args[i].IndexOf('\'', StringComparison.InvariantCulture) + 1) ..args[i].LastIndexOf('\'')];
            }

            // if they don't correspond
            if (properties.Length - propEmptyValues != values.Length - valEmptyValues)
            {
                return;
            }

            if (properties.Length - propEmptyValues != typeof(FileCabinetRecord).GetProperties().Length)
            {
                Console.WriteLine("Please, enter all the properties needed.");
            }

            PropertyInfo[] props = new PropertyInfo[properties.Length - propEmptyValues];

            // check if there are such properties
            for (int i = 0; i < properties.Length - propEmptyValues; i++)
            {
                PropertyInfo prop = typeof(FileCabinetRecord).GetProperty(properties[i], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (prop == null)
                {
                    // incorrect property
                    return;
                }
                else
                {
                    props[i] = prop;
                }
            }

            FileCabinetRecord record = new FileCabinetRecord();

            for (int i = 0; i < props.Length; i++)
            {
                // get the type of property
                Type type = props[i].PropertyType;

                dynamic value = null;

                // convert value to the type of property
                if (type.Name == "DateTime")
                {
                    try
                    {
                        value = DateTime.ParseExact(values[i], "M/d/yyyy", CultureInfo.InvariantCulture);
                    }
                    catch (FormatException)
                    {
                    }
                }
                else
                {
                    if (type.Name == "String")
                    {
                        value = values[i];
                    }
                    else
                    {
                        value = Convert.ChangeType(values[i], type, CultureInfo.InvariantCulture);
                    }
                }

                if (ReferenceEquals(null, value))
                {
                    Console.WriteLine("Convertion of " + props[i].Name + " failed.");
                    return;
                }

                props[i].SetValue(record, value);
            }

            int id = this.service.AddRecord(record);

            if (id == -1)
            {
                Console.WriteLine("Validation failed.");
            }
            else
            {
                Console.WriteLine($"Record {id} was inserted.");
            }
        }
    }
}
