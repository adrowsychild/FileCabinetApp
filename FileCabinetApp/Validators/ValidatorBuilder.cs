using System;
using System.Collections.Generic;
using System.Text;

namespace FileCabinetApp.Validators
{
    public class ValidatorBuilder
    {
        private readonly List<IRecordValidator> validators;

        public ValidatorBuilder ValidateFirstName(int min, int max)
        {
            this.validators.Add(new FirstNameValidator(min, max));

            return this;
        }

        public ValidatorBuilder ValidateLastName(int min, int max)
        {
            this.validators.Add(new LastNameValidator(min, max));

            return this;
        }

        public ValidatorBuilder ValidateDateOfBirth(int from, int to)
        {
            this.validators.Add(new FirstNameValidator(from, to));

            return this;
        }

        public ValidatorBuilder ValidateFavouriteNumber(short minValue, short maxValue)
        {
            this.validators.Add(new FavouriteNumberValidator(minValue, maxValue));

            return this;
        }

        public ValidatorBuilder ValidateFavouriteCharacter(int symbolCase)
        {
            this.validators.Add(new FavouriteCharacterValidator(symbolCase));

            return this;
        }

        public ValidatorBuilder ValidateFavouriteGame(int min, int max)
        {
            this.validators.Add(new FavouriteGameValidator(min, max));

            return this;
        }

        public ValidatorBuilder ValidateDonations(int minValue)
        {
            this.validators.Add(new DonationsValidator(minValue));

            return this;
        }

        public IRecordValidator Create()
        {
            return new CompositeValidator(this.validators);
        }
    }
}
