using System;
using System.Collections.Generic;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Validator builder.
    /// </summary>
    public class ValidatorBuilder
    {
        private readonly List<IRecordValidator> validators = new List<IRecordValidator>();

        /// <summary>
        /// Adds first name validator to the list of validators.
        /// </summary>
        /// <param name="min">Minimum possible length of first name.</param>
        /// <param name="max">Maximum possible length of first name.</param>
        /// <returns>The instance of ValidatorBuilder.</returns>
        public ValidatorBuilder ValidateFirstName(int min, int max)
        {
            this.validators.Add(new FirstNameValidator(min, max));

            return this;
        }

        /// <summary>
        /// Adds last name validator to the list of validators.
        /// </summary>
        /// <param name="min">Minimum possible length of last name.</param>
        /// <param name="max">Maximum possible length of last name.</param>
        /// <returns>The instance of ValidatorBuilder.</returns>
        public ValidatorBuilder ValidateLastName(int min, int max)
        {
            this.validators.Add(new LastNameValidator(min, max));

            return this;
        }

        /// <summary>
        /// Adds date of birth validator to the list of validators.
        /// </summary>
        /// <param name="from">Minimum possible age.</param>
        /// <param name="to">Maximum possible age.</param>
        /// <returns>The instance of ValidatorBuilder.</returns>
        public ValidatorBuilder ValidateDateOfBirth(int from, int to)
        {
            this.validators.Add(new FirstNameValidator(from, to));

            return this;
        }

        /// <summary>
        /// Adds favourite number validator to the list of validators.
        /// </summary>
        /// <param name="minValue">Minimum possible value.</param>
        /// <param name="maxValue">Maximum possible value.</param>
        /// <returns>The instance of ValidatorBuilder.</returns>
        public ValidatorBuilder ValidateFavouriteNumber(short minValue, short maxValue)
        {
            this.validators.Add(new FavouriteNumberValidator(minValue, maxValue));

            return this;
        }

        /// <summary>
        /// Adds favourite character validator to the list of validators.
        /// </summary>
        /// <param name="symbolCase">Case of character: 0 if case insensitive,
        /// -1 if only lowercase allowed, 1 if only uppercase allowed.</param>
        /// <returns>The instance of ValidatorBuilder.</returns>
        public ValidatorBuilder ValidateFavouriteCharacter(int symbolCase)
        {
            this.validators.Add(new FavouriteCharacterValidator(symbolCase));

            return this;
        }

        /// <summary>
        /// Adds favourite character validator to the list of validators.
        /// </summary>
        /// <param name="min">Minimum possible length of name of the game.</param>
        /// <param name="max">Maximum possible length of name of the game.</param>
        /// <returns>The instance of ValidatorBuilder.</returns>
        public ValidatorBuilder ValidateFavouriteGame(int min, int max)
        {
            this.validators.Add(new FavouriteGameValidator(min, max));

            return this;
        }

        /// <summary>
        /// Adds favourite character validator to the list of validators.
        /// </summary>
        /// <param name="minValue">Minimum possible value.</param>
        /// <returns>The instance of ValidatorBuilder.</returns>
        public ValidatorBuilder ValidateDonations(int minValue)
        {
            this.validators.Add(new DonationsValidator(minValue));

            return this;
        }

        /// <summary>
        /// Creates a validator.
        /// </summary>
        /// <returns>The instance of IRecordValidator class.</returns>
        public IRecordValidator Create()
        {
            return new CompositeValidator(this.validators);
        }
    }
}
