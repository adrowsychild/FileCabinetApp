using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the user's 'select' command.
    /// </summary>
    public class SelectCommandHandler : ServiceCommandHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectCommandHandler"/> class.
        /// </summary>
        /// <param name="fileCabinetService">Service to create record in.</param>
        public SelectCommandHandler(IFileCabinetService fileCabinetService)
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

            if (request.Command.ToLower() == "select")
            {
                this.Select(request.Parameters);
                return true;
            }
            else
            {
                return base.Handle(request);
            }
        }

        /// <summary>
        /// Writes values to the table.
        /// </summary>
        /// <param name="record">Record to write values from.</param>
        /// <param name="properties">Properties to write.</param>
        /// <param name="maxLength">Max lengths of the cells.</param>
        /// <param name="padding">Padding.</param>
        /// <returns>Outout string.</returns>
        private static string WriteValues(FileCabinetRecord record, List<PropertyInfo> properties, List<int> maxLength, int padding)
        {
            string output = string.Empty;
            dynamic value;
            output += '|';
            for (int i = 0; i < properties.Count; i++)
            {
                value = properties[i].GetValue(record);
                if (IsNumericType(value))
                {
                    // right align
                    output += WriteSpaces(maxLength[i] - (value.ToString().Length - 1));
                    output += value.ToString();
                    output += WriteSpaces(padding);
                    output += '|';
                }
                else
                {
                    // left align
                    output += WriteSpaces(padding);
                    int valueLength;
                    if (properties[i].PropertyType.Name == "DateTime")
                    {
                        output += value.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture);
                        valueLength = value.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture).Length;
                    }
                    else
                    {
                        output += value.ToString();
                        valueLength = value.ToString().Length;
                    }

                    output += WriteSpaces(maxLength[i] + padding - (padding + valueLength - 1));
                    output += '|';
                }
            }

            return output;
        }

        /// <summary>
        /// Writes names of properties to the table.
        /// </summary>
        /// <param name="properties">Properties to write.</param>
        /// <param name="maxLength">Max lengths of the cells.</param>
        /// <param name="padding">Padding.</param>
        /// <returns>Outout string.</returns>
        private static string WriteProperty(List<PropertyInfo> properties, List<int> maxLength, int padding)
        {
            string output = string.Empty;
            output += '|';
            string tempOutput = string.Empty;
            for (int i = 0; i < properties.Count; i++)
            {
                tempOutput = WriteSpaces(padding);
                tempOutput += properties[i].Name.ToString(CultureInfo.InvariantCulture);
                output += tempOutput;
                output += WriteSpaces(maxLength[i] + padding - (tempOutput.Length - 1));
                output += '|';
            }

            return output;
        }

        /// <summary>
        /// Writes white spaces.
        /// </summary>
        /// <param name="count">Number of spaces.</param>
        /// <returns>Output string in spaces.</returns>
        private static string WriteSpaces(int count)
        {
            string spaces = string.Empty;
            for (int i = 0; i < count; i++)
            {
                spaces += ' ';
            }

            return spaces;
        }

        /// <summary>
        /// Writes dashed line for all the properties.
        /// </summary>
        /// <param name="maxLength">Max lengths of the cells.</param>
        /// <param name="padding">Padding.</param>
        /// <returns>Output dashed line.</returns>
        private static string WriteDashedLine(List<int> maxLength, int padding)
        {
            string output = string.Empty;
            int space;
            output += "+";

            for (int i = 0; i < maxLength.Count; i++)
            {
                space = maxLength[i] + (padding * 2);
                output += WriteDashedLine(space + 1) + "+";
            }

            return output;
        }

        /// <summary>
        /// Writes only dashed part of line.
        /// </summary>
        /// <param name="length">Length of dashed line to write.</param>
        /// <returns>Output dashed line.</returns>
        private static string WriteDashedLine(int length)
        {
            string line = string.Empty;
            for (int i = 1; i < length; i++)
            {
                if (i % 2 == 0)
                {
                    line += '-';
                }
                else
                {
                    line += ' ';
                }
            }

            return line;
        }

        /// <summary>
        /// Checks whether object is of numeric type.
        /// </summary>
        /// <param name="o">Object to check type of.</param>
        /// <returns>Whether object is of numeric type.</returns>
        private static bool IsNumericType(object o)
        {
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Selects the records and shows them to the user.
        /// </summary>
        /// <param name="parameters">Parameters.</param>
        private void Select(string parameters)
        {
            int indexOfWhere = parameters.IndexOf("where", StringComparison.OrdinalIgnoreCase);
            if (indexOfWhere == -1)
            {
                return;
            }

            List<PropertyInfo> propsToShow;

            if (parameters.StartsWith("where", StringComparison.OrdinalIgnoreCase))
            {
                propsToShow = null;
            }
            else
            {
                propsToShow = ParseProperties(parameters.Substring(0, indexOfWhere - 1));
                if (propsToShow == null)
                {
                    return;
                }
            }

            ReadOnlyCollection<FileCabinetRecord> foundRecords;

            if (propsToShow == null)
            {
                propsToShow = typeof(FileCabinetRecord).GetProperties().ToList();
            }

            if (indexOfWhere + 5 != parameters.Length)
            {
                foundRecords = this.WhereParser(parameters.Substring(indexOfWhere + 6).Split());

                if (foundRecords == null)
                {
                    return;
                }

                if (foundRecords.Count == 0)
                {
                    Console.WriteLine("No records found.");
                    return;
                }
            }
            else
            {
                Console.WriteLine("No 'where' found. All the records:");
                foundRecords = this.Service.GetRecords();
            }

            int[] maxLength = new int[propsToShow.Count];
            dynamic value;
            int valueLength;
            int propertyLength;
            int tempMaxLength;

            for (int i = 0; i < foundRecords.Count; i++)
            {
                for (int j = 0; j < propsToShow.Count; j++)
                {
                    tempMaxLength = 0;
                    value = propsToShow[j].GetValue(foundRecords[i]);
                    if (propsToShow[j].PropertyType.Name == "DateTime")
                    {
                        valueLength = value.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture).Length;
                    }
                    else
                    {
                        valueLength = value.ToString().Length;
                    }

                    propertyLength = propsToShow[j].Name.Length;
                    tempMaxLength = propertyLength > valueLength ? propertyLength : valueLength;
                    maxLength[j] = maxLength[j] > tempMaxLength ? maxLength[j] : tempMaxLength;
                    maxLength[j] = tempMaxLength;
                }
            }

            int padding = 1;

            Console.WriteLine(WriteDashedLine(maxLength.ToList(), padding));
            Console.WriteLine(WriteProperty(propsToShow, maxLength.ToList(), padding));
            Console.WriteLine(WriteDashedLine(maxLength.ToList(), padding));

            foreach (var record in foundRecords)
            {
                Console.WriteLine(WriteValues(record, propsToShow, maxLength.ToList(), padding));
                Console.WriteLine(WriteDashedLine(maxLength.ToList(), padding));
            }
        }
    }
}