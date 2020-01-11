using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace FileCabinetApp.Helpers
{
    /// <summary>
    /// Class for helping working with bytes.
    /// </summary>
    public static class WorkWithBytesHelper
    {
        /// <summary>
        /// Makes an offset for the string.
        /// </summary>
        /// <param name="value">String to make offset to.</param>
        /// <param name="offset">Overall number of bytes.</param>
        /// <returns>String with offset in bytes.</returns>
        public static byte[] MakeStringOffset(string value, int offset)
        {
            byte[] bytes = new byte[offset];
            byte[] somebytes = Encoding.ASCII.GetBytes(value);
            for (int i = 0; i < somebytes.Length; i++)
            {
                bytes[i] = somebytes[i];
            }

            return bytes;
        }

        /// <summary>
        /// Removes the offset from the string.
        /// </summary>
        /// <param name="value">String to remove offset from.</param>
        /// <returns>String without offset.</returns>
        public static string RemoveOffset(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException($"Value is null.");
            }

            int i = 0;
            int actualLength = 1;

            while (i < value.Length && value[i] != 0x0)
            {
                actualLength++;
                i++;
            }

            value = value.Substring(0, i);

            return value;
        }

        /// <summary>
        /// Converts decimal value to array of bytes.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>Array of bytes.</returns>
        public static byte[] DecimalToBytes(decimal value)
        {
            byte[] bytes = new byte[sizeof(decimal)];

            using (MemoryStream stream = new MemoryStream())
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    writer.Write(value);
                    bytes = stream.ToArray();
                }
            }

            return bytes;
        }

        /// <summary>
        /// Converts array of bytes to decimal value.
        /// </summary>
        /// <param name="bytes">Bytes to convert.</param>
        /// <returns>Decimal value.</returns>
        public static decimal BytesToDecimal(byte[] bytes)
        {
            decimal value;

            using (MemoryStream stream = new MemoryStream(bytes))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    value = reader.ReadDecimal();
                }
            }

            return value;
        }
    }
}
