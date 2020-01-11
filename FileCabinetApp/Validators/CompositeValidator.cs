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
        /// <returns>Whether record is valid.</returns>
        public bool Validate(FileCabinetRecord record)
        {
            bool isValid = false;

            foreach (var validator in this.validators)
            {
                isValid = validator.Validate(record);
            }

            return isValid;
        }
    }
}
