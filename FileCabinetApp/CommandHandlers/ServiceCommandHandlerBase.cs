using System;
using System.Collections.Generic;
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
        /// The service to get information and modify.
        /// </summary>
        protected IFileCabinetService service;

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
                exceptionMessage = this.service.GetValidator().Validate(record);
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

        protected Tuple<string, string, int> ParsePropertyValuePair(string[] args)
        {
            // tuple
            // string property
            // string value
            // int size

            string property;
            string value;
            int size;
            int equalsIndex = -1;

            int maxEqualIndex = args.Length < 3 ? args.Length : 3;

            for (int i = 0; i < maxEqualIndex; i++)
            {
                if (args[i].Contains('='))
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
                // id = '1'
                if (args[1] == "=")
                {
                    value = args[2];
                    size = 3;
                }
                else
                {
                    // id ='1'
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
                // id= '1'
                if (args[0].EndsWith('='))
                {
                    property = args[1].Substring(0, args[1].Length - 1);
                    value = args[2];
                    size = 2;
                }
                else // where id='1'
                {
                    string[] subArgs = args[0].Split('=');
                    value = subArgs[1];
                    property = subArgs[0];
                    size = 1;
                }
            }

            if (!value.Contains('\'') || (value.IndexOf('\'', StringComparison.InvariantCulture) == value.LastIndexOf('\'')))
            {
                return null;
            }
            else
            {
                value = value[(value.IndexOf('\'', StringComparison.InvariantCulture) + 1)..value.LastIndexOf('\'')];
            }

            return new Tuple<string, string, int>(property, value, size);
        }

        protected string[] SubArrString(string[] arr, int from)
        {
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

        protected string[] SubArrString(string[] arr, int from, int to)
        {
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

        protected List<FileCabinetRecord> WhereParser(string[] args)
        {
            int numOfOr = 0;

            foreach (string s in args)
            {
                if (s.ToLower().Contains("or"))
                {
                    numOfOr++;
                }
            }

            int numOfSets = numOfOr + 1;
            int numOfProps = typeof(FileCabinetRecord).GetProperties().Length;

            // contains values (ray, archie, gray)
            string[] sets = new string[numOfSets * numOfProps];

            // contains records found in different sets
            List<FileCabinetRecord> foundRecords = new List<FileCabinetRecord>();

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
                args = this.SubArrString(args, jump);
                Tuple<string, string, int> propValue = this.ParsePropertyValuePair(args);

                if (propValue == null)
                {
                    return null;
                }

                // we have string method string value
                if (!propIndex.ContainsKey(propValue.Item1.ToLower()))
                {
                    // incorrect property
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
                                MethodInfo findMethod = this.service.GetType().GetMethod("FindBy" + properties[y].Name);
                                if (findMethod == null)
                                {
                                    // incorrect name of property
                                    return null;
                                }
                                else
                                {
                                    try
                                    {
                                        if (this.service is FileCabinetFilesystemService)
                                        {
                                            FileSystemRecordEnumerator enumeratedrecords = (FileSystemRecordEnumerator)findMethod.Invoke(this.service, new object[] { sets[x + y] });
                                            tempRecords = FileSystemRecordEnumerator.ToList(enumeratedrecords);
                                        }
                                        else
                                        {
                                            tempRecords = (List<FileCabinetRecord>)findMethod.Invoke(this.service, new object[] { sets[x + y] });
                                        }

                                        if (tempRecords.Count != 0)
                                        {
                                            // conditions store foundRecords of the set -> one set of records + condition
                                            conditions.Add(tempRecords, x + y);
                                        }
                                    }
                                    catch (ArgumentException ex)
                                    {
                                        Console.WriteLine(ex.Message);
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
                        // we have single-condition set of records
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

                    return foundRecords;
                }

                if (args[propValue.Item3].Contains("or"))
                {
                    currentSet++;
                }
                else if (args[propValue.Item3].Contains("and"))
                {
                }
                else
                {
                    return null;
                }

                jump = propValue.Item3 + 1;
                i += jump;
            }

            return foundRecords;
        }

        protected string[] MakeSet(string parameters)
        {
            int indexOfWhere = parameters.IndexOf("where");
            if (indexOfWhere == -1)
            {
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
            if (!itemsToSet.Contains(','))
            {
                propValue = ParsePropertyValuePair(itemsToSet.Substring(1).Split());
                if (propValue == null)
                {
                    return null;
                }

                // check if such property exists
                if (!propIndex.ContainsKey(propValue.Item1.ToLower()))
                {
                    // incorrect property
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

        protected List<PropertyInfo> ParseProperties(string parameters)
        {
            List<PropertyInfo> propsToShow = new List<PropertyInfo>();
            Tuple<string, string, int> propValue;

            // if there is only one property
            if (!parameters.Contains(','))
            {
                if (parameters.StartsWith(' '))
                {
                    parameters = parameters.Substring(1);
                }

                PropertyInfo property = typeof(FileCabinetRecord).GetProperty(parameters, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                
                // check if such property exists
                if (property == null)
                {
                    // incorrect property
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
                        // incorrect property
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
    }
}
