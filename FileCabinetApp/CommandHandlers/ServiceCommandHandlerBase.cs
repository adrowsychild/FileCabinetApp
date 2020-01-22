using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Reflection;

namespace FileCabinetApp.CommandHandlers
{
    /// <summary>
    /// Handler for the user's commands which relate to the service.
    /// </summary>
    public abstract class ServiceCommandHandlerBase : CommandHandlerBase
    {
        /// <summary>
        /// Gets or sets the service to get information from and modify.
        /// </summary>
        /// <value>Service to get information from and modify.</value>
        protected IFileCabinetService Service { get; set; }

        /// <summary>
        /// Parses and validates the user's input.
        /// </summary>
        /// <typeparam name="T">The type of input.</typeparam>
        /// <param name="converter">Converter from string to specific type.</param>
        /// <returns>Converted and validated value.</returns>
        protected static T ReadInput<T>(Func<string, Tuple<bool, string, T>> converter)
        {
            do
            {
                T value;

                var input = Console.ReadLine();
                var conversionResult = converter(input);

                if (!conversionResult.Item1)
                {
                    Console.WriteLine($"Conversion failed: {conversionResult.Item2}. Please, correct your input.");
                    continue;
                }

                value = conversionResult.Item3;

                return value;
            }
            while (true);
        }

        /// <summary>
        /// Converts the value to specific type.
        /// </summary>
        /// <typeparam name="T">The type to convert to.</typeparam>
        /// <param name="input">The input to convert.</param>
        /// <returns>
        /// Bool whether the conversion succeeded.
        /// Type of conversion as string.
        /// Converted value itself.
        /// </returns>
        protected static Tuple<bool, string, T> Converter<T>(string input)
        {
            bool conversionSucceeded = false;
            string conversionType = $"from string to {typeof(T)}";
            T convertedValue = default;

            try
            {
                convertedValue = (T)Convert.ChangeType(input, typeof(T), CultureInfo.InvariantCulture);
                conversionSucceeded = true;
            }
            catch (FormatException)
            {
            }

            return new Tuple<bool, string, T>(conversionSucceeded, conversionType, convertedValue);
        }

        /// <summary>
        /// Parses the string to tuple.
        /// </summary>
        /// <param name="args">String to parse.</param>
        /// <returns>Tuple of string name of property,
        /// string value to find by and size that the condition takes in string.</returns>
        protected static Tuple<string, string, int> ParsePropertyValuePair(string[] args)
        {
            if (args == null)
            {
                return null;
            }

            string property;
            string value;
            int size;
            int equalsIndex = -1;

            int maxEqualIndex = args.Length < 3 ? args.Length : 3;

            for (int i = 0; i < maxEqualIndex; i++)
            {
                if (args[i].Contains('=', StringComparison.InvariantCulture))
                {
                    equalsIndex = i;
                    break;
                }
            }

            if (equalsIndex == -1 || equalsIndex >= 3)
            {
                return null;
            }

            if (equalsIndex == 1)
            {
                if (args[1] == "=")
                {
                    value = args[2];
                    size = 3;
                }
                else
                {
                    if (args[1].StartsWith('='))
                    {
                        value = args[1].Substring(0);
                        size = 2;
                    }
                    else
                    {
                        return null;
                    }
                }

                property = args[0];
            }
            else
            {
                if (args[0].EndsWith('='))
                {
                    property = args[1].Substring(0, args[1].Length - 1);
                    value = args[2];
                    size = 2;
                }
                else
                {
                    string[] subArgs = args[0].Split('=');
                    value = subArgs[1];
                    property = subArgs[0];
                    size = 1;
                }
            }

            if (!value.Contains('\'', StringComparison.InvariantCulture) || (value.IndexOf('\'', StringComparison.InvariantCulture) == value.LastIndexOf('\'')))
            {
                return null;
            }
            else
            {
                value = value[(value.IndexOf('\'', StringComparison.InvariantCulture) + 1) ..value.LastIndexOf('\'')];
            }

            return new Tuple<string, string, int>(property, value, size);
        }

        /// <summary>
        /// Extracts part array of string from array of string.
        /// </summary>
        /// <param name="arr">Array to extract from.</param>
        /// <param name="from">Index to start from.</param>
        /// <returns>Extracted string.</returns>
        protected static string[] SubArrString(string[] arr, int from)
        {
            if (arr == null)
            {
                return null;
            }

            // if from == 0 return arr
            if (from == 0)
            {
                return arr;
            }

            string[] newArr = new string[arr.Length - from];
            for (int i = from; i < arr.Length; i++)
            {
                newArr[i - from] = arr[i];
            }

            return newArr;
        }

        /// <summary>
        /// Extracts part array of string from array of string.
        /// </summary>
        /// <param name="arr">Array to extract from.</param>
        /// <param name="from">Index to start from.</param>
        /// <param name="to">Index to extract to.</param>
        /// <returns>Extracted string.</returns>
        protected static string[] SubArrString(string[] arr, int from, int to)
        {
            if (arr == null)
            {
                return null;
            }

            // if from == 0 return arr
            if (from == 0)
            {
                return arr;
            }

            string[] newArr = new string[arr.Length - from];
            for (int i = from; i < to; i++)
            {
                newArr[i - from] = arr[i];
            }

            return newArr;
        }

        /// <summary>
        /// Parses the list of properties in string(possibly separated by comas).
        /// </summary>
        /// <param name="parameters">Parameters to parse by.</param>
        /// <returns>The list of properties.</returns>
        protected static List<PropertyInfo> ParseProperties(string parameters)
        {
            if (parameters == null)
            {
                return null;
            }

            List<PropertyInfo> propsToShow = new List<PropertyInfo>();

            // if there is only one property
            if (!parameters.Contains(',', StringComparison.InvariantCulture))
            {
                if (parameters.StartsWith(' '))
                {
                    parameters = parameters.Substring(1);
                }

                PropertyInfo property = typeof(FileCabinetRecord).GetProperty(parameters, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                // check if such property exists
                if (property == null)
                {
                    Console.WriteLine("Incorrect name of property.");
                    return null;
                }
                else
                {
                    propsToShow.Add(property);
                }

                return propsToShow;
            }
            else
            {
                string[] args = parameters.Split(',');
                int numOfConditions = args.Length;

                for (int i = 0; i < numOfConditions; i++)
                {
                    if (args[i].StartsWith(' '))
                    {
                        args[i] = args[i].Substring(1);
                    }

                    // add case invariant
                    PropertyInfo property = typeof(FileCabinetRecord).GetProperty(args[i], BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

                    // check if such property exists
                    if (property == null)
                    {
                        Console.WriteLine("Incorrect name of property.");
                        return null;
                    }
                    else
                    {
                        propsToShow.Add(property);
                    }
                }

                return propsToShow;
            }
        }

        /// <summary>
        /// Makes set of property-value.
        /// </summary>
        /// <param name="parameters">Arrays of values.</param>
        /// <returns>Arrays of values in special corresponding with properties cells.</returns>
        protected static string[] MakeSet(string parameters)
        {
            if (parameters == null)
            {
                return null;
            }

            int indexOfWhere = parameters.IndexOf("where", StringComparison.OrdinalIgnoreCase);
            if (indexOfWhere == -1)
            {
                Console.WriteLine("Command should contain 'where'.");
                return null;
            }

            int numOfProps = typeof(FileCabinetRecord).GetProperties().Length;
            string[] sets = new string[numOfProps];

            string itemsToSet = parameters.Substring(0, indexOfWhere - 1);
            Tuple<string, string, int> propValue;

            // propertyName and its index in the dictionary
            Dictionary<string, int> propIndex = new Dictionary<string, int>();

            var properties = typeof(FileCabinetRecord).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                // set the indexes
                propIndex.Add(properties[i].Name.ToLower(), i);
            }

            // if there is only one property
            if (!itemsToSet.Contains(',', StringComparison.InvariantCulture))
            {
                propValue = ParsePropertyValuePair(itemsToSet.Substring(1).Split());
                if (propValue == null)
                {
                    return null;
                }

                // check if such property exists
                if (!propIndex.ContainsKey(propValue.Item1.ToLower()))
                {
                    Console.WriteLine("Incorrect name of property.");
                    return null;
                }
                else
                {
                    sets[propIndex[propValue.Item1.ToLower()]] = propValue.Item2;
                }

                return sets;
            }
            else
            {
                string[] args = itemsToSet.Split(',');
                int numOfConditions = args.Length;

                for (int i = 0; i < numOfConditions; i++)
                {
                    string[] tempArgs = args[i].Substring(1).Split();
                    propValue = ParsePropertyValuePair(tempArgs);

                    if (propValue == null)
                    {
                        return null;
                    }

                    // we have string method string value
                    if (!propIndex.ContainsKey(propValue.Item1.ToLower()))
                    {
                        // incorrect property
                        Console.WriteLine("Incorrect name of property.");
                        return null;
                    }
                    else
                    {
                        sets[propIndex[propValue.Item1.ToLower()]] = propValue.Item2;
                    }
                }

                return sets;
            }
        }

        /// <summary>
        /// Parses the criterion after 'where'.
        /// </summary>
        /// <param name="args">Strings to parse.</param>
        /// <returns>Collection of records.</returns>
        protected ReadOnlyCollection<FileCabinetRecord> WhereParser(string[] args)
        {
            if (args == null)
            {
                return null;
            }

            ReadOnlyCollection<FileCabinetRecord> result;
            List<FileCabinetRecord> foundRecords = new List<FileCabinetRecord>();

            if (string.IsNullOrWhiteSpace(args[0]) || string.IsNullOrEmpty(args[0]))
            {
                Console.WriteLine("No 'where' found. All the records:");
                result = new ReadOnlyCollection<FileCabinetRecord>(this.Service.GetRecords());
                return result;
            }

            int numOfOr = 0;

            foreach (string s in args)
            {
                if (s.ToLower().Contains("or", StringComparison.InvariantCulture))
                {
                    numOfOr++;
                }
            }

            int numOfSets = numOfOr + 1;
            int numOfProps = typeof(FileCabinetRecord).GetProperties().Length;

            // contains values (ray, archie, gray)
            string[] sets = new string[numOfSets * numOfProps];

            // propertyName and its index in the dictionary
            Dictionary<string, int> propIndex = new Dictionary<string, int>();

            var properties = typeof(FileCabinetRecord).GetProperties();
            for (int i = 0; i < properties.Length; i++)
            {
                // set the indexes
                propIndex.Add(properties[i].Name.ToLower(), i);
            }

            int currentSet = 0;
            int jump = 0;

            int argsLen = args.Length;

            for (int i = 0; i < argsLen;)
            {
                args = SubArrString(args, jump);
                Tuple<string, string, int> propValue = ParsePropertyValuePair(args);

                if (propValue == null)
                {
                    return null;
                }

                // we have string method string value
                if (!propIndex.ContainsKey(propValue.Item1.ToLower()))
                {
                    Console.WriteLine("Incorrect name of property.");
                    return null;
                }
                else
                {
                    sets[propIndex[propValue.Item1.ToLower()] + (numOfProps * currentSet)] = propValue.Item2;
                }

                // where all the conditions are read
                if (argsLen - i - propValue.Item3 == 0)
                {
                    // final logic and exit
                    for (int x = 0; x < numOfSets * numOfProps; x += numOfProps)
                    {
                        // contains a set of found records
                        List<FileCabinetRecord> tempRecords = new List<FileCabinetRecord>();
                        Dictionary<List<FileCabinetRecord>, int> conditions = new Dictionary<List<FileCabinetRecord>, int>();

                        for (int y = 0; y < numOfProps; y++)
                        {
                            if (sets[x + y] != null)
                            {
                                MethodInfo findMethod = this.Service.GetType().GetMethod("FindBy" + properties[y].Name);
                                if (findMethod == null)
                                {
                                    Console.WriteLine("Incorrect name of property.");
                                    return null;
                                }
                                else
                                {
                                    IEnumerable<FileCabinetRecord> records = (IEnumerable<FileCabinetRecord>)findMethod.Invoke(this.Service, new object[] { sets[x + y] });
                                    if (records is FileSystemRecordCollection)
                                    {
                                        tempRecords = FileSystemRecordCollection.ToList((FileSystemRecordCollection)records);
                                    }
                                    else
                                    {
                                        tempRecords = (List<FileCabinetRecord>)records;
                                    }

                                    if (tempRecords == null)
                                    {
                                        Console.WriteLine("No records found.");
                                        return null;
                                    }

                                    if (tempRecords.Count != 0)
                                    {
                                        conditions.Add(tempRecords, x + y);
                                    }
                                }
                            }
                        }

                        if (conditions.Count > 1)
                        {
                            // we have to sum the conditions and find records to delete
                            List<FileCabinetRecord> recordsToDelete = new List<FileCabinetRecord>();

                            // for each condition in dictionary
                            foreach (var pair in conditions)
                            {
                                // find property to check
                                PropertyInfo prop = properties[pair.Value - x];

                                foreach (var recordSet in conditions.Keys)
                                {
                                    foreach (var record in recordSet)
                                    {
                                        // find records which do not match
                                        if (prop.GetValue(record).ToString() != sets[pair.Value])
                                        {
                                            recordsToDelete.Add(record);
                                        }
                                    }
                                }
                            }

                            // delete dismatched records
                            for (int q = 0; q < recordsToDelete.Count; q++)
                            {
                                foreach (var pair in conditions)
                                {
                                    pair.Key.Remove(recordsToDelete[q]);
                                }
                            }

                            // add records to final list without duplicates
                            foreach (var recordSet in conditions.Keys)
                            {
                                foreach (var record in recordSet)
                                {
                                    if (!foundRecords.Contains(record))
                                    {
                                        foundRecords.Add(record);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (conditions.Count != 0)
                            {
                                foreach (var key in conditions.Keys)
                                {
                                    foreach (var record in key)
                                    {
                                        if (!foundRecords.Contains(record))
                                        {
                                            foundRecords.Add(record);
                                        }
                                    }
                                }
                            }
                        }
                    }

                    return new ReadOnlyCollection<FileCabinetRecord>(foundRecords);
                }

                if (args[propValue.Item3].Contains("or", StringComparison.InvariantCulture))
                {
                    currentSet++;
                }
                else if (args[propValue.Item3].Contains("and", StringComparison.InvariantCulture))
                {
                }
                else
                {
                    return null;
                }

                jump = propValue.Item3 + 1;
                i += jump;
            }

            return new ReadOnlyCollection<FileCabinetRecord>(foundRecords);
        }

        /// <summary>
        /// Requests and checks the user's input.
        /// </summary>
        /// <param name="id">Id to create the record with.</param>
        /// <returns>Valid record.</returns>
        protected FileCabinetRecord CheckRecordInput(int id = 1)
        {
            FileCabinetRecord record = null;
            string exceptionMessage = null;

            do
            {
                Console.WriteLine("First Name: ");
                string tmpFirstName = ReadInput(Converter<string>);

                Console.WriteLine("Last Name: ");
                string tmpLastName = ReadInput(Converter<string>);

                Console.WriteLine("Date of Birth: ");
                DateTime tmpDateOfBirth = ReadInput(Converter<DateTime>);

                Console.WriteLine("Favourite number: ");
                short tmpFavouriteNumber = ReadInput(Converter<short>);

                Console.WriteLine("Favourite character: ");
                char tmpFavouriteCharacter = ReadInput(Converter<char>);

                Console.WriteLine("Favourite game: ");
                string tmpFavouriteGame = ReadInput(Converter<string>);

                Console.WriteLine("Donations: ");
                decimal tmpDonations = ReadInput(Converter<decimal>);

                record = new FileCabinetRecord(id, tmpFirstName, tmpLastName, tmpDateOfBirth, tmpFavouriteNumber, tmpFavouriteCharacter, tmpFavouriteGame, tmpDonations);
                exceptionMessage = this.Service.GetValidator().Validate(record);
                if (exceptionMessage != null)
                {
                    Console.WriteLine("Validation failed:");
                    Console.WriteLine(exceptionMessage);
                    Console.WriteLine("Please Try again.");
                }
            }
            while (exceptionMessage != null);

            return record;
        }
    }
}