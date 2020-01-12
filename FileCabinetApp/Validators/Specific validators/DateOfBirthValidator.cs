using System;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Custom date of birth validator.
    /// </summary>
    public class DateOfBirthValidator : IRecordValidator
    {
        private readonly int from;
        private readonly int to;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateOfBirthValidator"/> class.
        /// </summary>
        /// <param name="from">Minimum possible age.</param>
        /// <param name="to">Maximum possible age.</param>
        public DateOfBirthValidator(int from, int to)
        {
            this.from = from;
            this.to = to;
        }

        /// <summary>
        /// Validates date of birth of user's input.
        /// </summary>
        /// <param name="record">Record to validate.</param>
        /// <returns>Exception message.</returns>
        public string Validate(FileCabinetRecord record)
        {
            if (record == null)
            {
                throw new ArgumentNullException($"Record is invalid.");
            }

            if ((DateTime.Today.Year - record.DateOfBirth.Year) < this.from || (DateTime.Today.Year - record.DateOfBirth.Year) > this.to)
            {
                return "Date of birth is invalid. Should be from " + this.from + " to " + this.to + " years old.";
            }

            return null;
        }
    }
}
