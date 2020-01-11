using System;
using System.Collections.Generic;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Composite validator.
    /// </summary>
    public class CompositeValidator : IRecordValidator
    {
        private readonly List<IRecordValidator> validators;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeValidator"/> class.
        /// </summary>
        /// <param name="validators">List of validators.</param>
        public CompositeValidator(IEnumerable<IRecordValidator> validators)
        {
            this.validators = (List<IRecordValidator>)validators;
        }

        /// <summary>
        /// Validates user's input.
        /// </summary>
        /// <param name="record">Record to validate.</param>
        /// <returns>Exception message.</returns>
        public string Validate(FileCabinetRecord record)
        {
            string exceptionMessage = null;
            string tmpExceptionMessage;

            foreach (var validator in this.validators)
            {
                tmpExceptionMessage = validator.Validate(record);
                if (tmpExceptionMessage != null)
                {
                    exceptionMessage += tmpExceptionMessage;
                    exceptionMessage += "\n";
                    tmpExceptionMessage = null;
                }
            }

            return exceptionMessage;
        }
    }
}
