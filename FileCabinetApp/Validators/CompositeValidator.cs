using System;
using System.Collections.Generic;

namespace FileCabinetApp.Validators
{
    public abstract class CompositeValidator : IRecordValidator
    {
        private readonly List<IRecordValidator> validators;

        protected CompositeValidator(IEnumerable<IRecordValidator> validators)
        {
            this.validators = (List<IRecordValidator>)validators;
        }

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
