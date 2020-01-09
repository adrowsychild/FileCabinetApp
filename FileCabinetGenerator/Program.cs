using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using FileCabinetApp;

namespace FileCabinetGenerator
{
    public class Program
    {
        private static string OutputType;
        private static string OutputFileName;
        private static int RecordsAmount;
        private static int StartId;

        /// <summary>
        /// Contains the name of operation and corresponding operation itself.
        /// Then, if we need to add settings, we only need to add values to the dictionary.
        /// Warning: Both forms of command should be added one after another.
        /// </summary>
        private static readonly Dictionary<string, Settings> ChangeSettings = new Dictionary<string, Settings>
        {
            ["--output-type"] = new Settings(SetOutputType),
            ["-t"] = new Settings(SetOutputType),
            ["--output"] = new Settings(SetOutput),
            ["-o"] = new Settings(SetOutput),
            ["--records-amount"] = new Settings(SetRecordsAmount),
            ["-a"] = new Settings(SetRecordsAmount),
            ["--start-id"] = new Settings(SetStartId),
            ["-i"] = new Settings(SetStartId),
        };

        private delegate void Settings(string args);

        static void Main(string[] args)
        {
            if (args == null)
            {
                Console.WriteLine("Enter the parameters: output-type, output, records-amount and start-id");
            }

            if (args != null && args.Length > 0)
            {
                string[] tempArgs = new string[2];

                for (int i = 1; i <= args.Length;)
                {
                    tempArgs[0] = args[i - 1];
                    if (i != args.Length)
                    {
                        tempArgs[1] = args[i];
                    }

                    int parsedSetting = SettingsParser(tempArgs);
                    if (parsedSetting == 0)
                    {
                        break;
                    }
                    else if (parsedSetting == 1)
                    {
                        i++;
                    }
                    else if (parsedSetting == 2)
                    {
                        i += 2;
                    }
                }
            }

            IRecordGenerator generator = new DefaultRecordGenerator();

            List<FileCabinetRecord> records = generator.GenerateRecords(StartId, RecordsAmount);

            WriteToCsv(records, "file.csv");
            WriteToXml(records, "file.xml");

            Console.WriteLine(RecordsAmount + " records were written to " + OutputFileName + ".");

            Console.ReadKey();
        }

        /// <summary>
        /// Writes records to the csv file.
        /// </summary>
        /// <param name="records">Records to write.</param>
        /// <param name="path">Path to write to.</param>
        private static void WriteToCsv(List<FileCabinetRecord> records, string path)
        {
            using (StreamWriter writer = new StreamWriter(path))
            {
                foreach (var record in records)
                {
                    PropertyInfo[] properties = record.GetType().GetProperties();

                    for (int i = 0; i < properties.Length; i++)
                    {
                        if (properties[i].PropertyType == typeof(DateTime))
                        {
                            DateTime date = (DateTime)properties[i].GetValue(record);
                            writer.Write(date.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture));
                        }
                        else
                        {
                            writer.Write(properties[i].GetValue(record));
                        }

                        if (i != properties.Length - 1)
                        {
                            writer.Write(',');
                        }
                    }

                    writer.Write("\n");
                }
            }
        }

        private static void WriteToXml(List<FileCabinetRecord> records, string path)
        {
            using (XmlWriter writer = XmlWriter.Create(new StreamWriter(path)))
            {
                XmlSerializer ser = new XmlSerializer(typeof(List<FileCabinetRecord>));

                ser.Serialize(writer, records);
            }
        }

        /// <summary>
        /// Shows one record.
        /// </summary>
        /// <param name="record">Record to show.</param>
        private static void ShowRecord(FileCabinetRecord record)
        {
            string output = "#";
            PropertyInfo[] properties = record.GetType().GetProperties();

            for (int i = 0; i < properties.Length; i++)
            {
                if (properties[i].PropertyType == typeof(DateTime))
                {
                    DateTime date = (DateTime)properties[i].GetValue(record);
                    output += "Date of birth" + ": ";
                    output += date.ToString("yyyy-MMM-d", CultureInfo.InvariantCulture);
                }
                else
                {
                    if (properties[i].Name != "Id")
                    {
                        output += properties[i].Name + ": ";
                    }

                    output += properties[i].GetValue(record);
                }

                if (i != properties.Length - 1)
                {
                    output += ", ";
                }
            }

            Console.WriteLine(output);
        }

        /// <summary>
        /// Parses the settings from application arguments
        /// and searches for given operation in the dictionary.
        /// </summary>
        /// <param name="args">Arguments to parse.</param>
        private static int SettingsParser(string[] args)
        {
            string operation = string.Empty;
            string parameter = string.Empty;
            Settings makeChanges = null;
            int parsedSetting = 0;

            // --some-operation=paramater
            if (args[0].StartsWith("--", StringComparison.InvariantCulture))
            {
                int index = args[0].IndexOf("=", StringComparison.InvariantCulture);
                if (index != -1)
                {
                    operation = args[0].Substring(0, index);
                    parameter = args[0].Substring(index + 1);
                    if (ChangeSettings.ContainsKey(operation.ToLower()))
                    {
                        makeChanges = ChangeSettings[operation.ToLower()];
                        parsedSetting = 1;
                    }
                }
            }

            // -o parameter
            else if (args[0].StartsWith("-", StringComparison.InvariantCulture))
            {
                operation = args[0];
                if (args[1] != null)
                {
                    parameter = args[1];
                }

                if (ChangeSettings.ContainsKey(operation.ToLower()))
                {
                    makeChanges = ChangeSettings[operation.ToLower()];
                    parsedSetting = 2;
                }
            }

            if (!string.IsNullOrEmpty(operation) && !string.IsNullOrEmpty(parameter) && makeChanges != null)
            {
                makeChanges.Invoke(parameter.ToLower());
            }

            return parsedSetting;
        }

        /// <summary>
        /// Sets the output type.
        /// </summary>
        /// <param name="outputType">Output type to set.</param>
        private static void SetOutputType(string outputType)
        {
            if (outputType == "csv" || outputType == "xml")
            {
                OutputType = outputType;
            }
            else
            {
                Console.WriteLine("Invalid output type. Should be csv or xml.");
            }
        }

        /// <summary>
        /// Sets the fileName.
        /// </summary>
        /// <param name="fileName">fileName to set.</param>
        private static void SetOutput(string fileName)
        {
            if (OutputType == null)
            {
                Console.WriteLine("Set output type before output filename.");
                return;
            }

            if (fileName.EndsWith("." + OutputType))
            {
                OutputFileName = fileName;
            }
            else
            {
                Console.WriteLine("Invalid file name.");
            }
        }

        /// <summary>
        /// Sets the amount of records to write.
        /// </summary>
        /// <param name="recordsAmount">Amount of records.</param>
        private static void SetRecordsAmount(string recordsAmount)
        {
            if (int.TryParse(recordsAmount, out int amount))
            {
                if (amount > 0)
                {
                    RecordsAmount = amount;
                }
                else
                {
                    Console.WriteLine("Invalid amount of records. Should be more than 0.");
                }
            }
            else
            {
                Console.WriteLine("Invalid amount of records. Should be integer.");
            }
        }

        /// <summary>
        /// Sets the start-id.
        /// </summary>
        /// <param name="startId">Start-id</param>
        private static void SetStartId(string startId)
        {
            if (int.TryParse(startId, out int amount))
            {
                if (amount >= 0)
                {
                    StartId = amount;
                }
                else
                {
                    Console.WriteLine("Invalid amount of records. Should be non-negative.");
                }
            }
            else
            {
                Console.WriteLine("Invalid start id. Should be integer.");
            }
        }
    }
}
