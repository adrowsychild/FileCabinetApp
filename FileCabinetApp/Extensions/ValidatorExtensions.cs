using System;
using System.Collections.Generic;
using FileCabinetApp.Validators;

namespace FileCabinetApp.Extensions
{
    /// <summary>
    /// Extends ValidatorBuilder class.
    /// </summary>
    public static class ValidatorExtensions
    {
        /// <summary>
        /// Creates default validator.
        /// </summary>
        /// <param name="validatorBuilder">The instance of ValidatorBuilder class.</param>
        /// <returns>The instance of IRecordValidator class.</returns>
        public static IRecordValidator CreateDefault(this ValidatorBuilder validatorBuilder)
        {
            return new CompositeValidator(new List<IRecordValidator>
            {
                new FirstNameValidator(2, 60),
                new LastNameValidator(2, 60),
                new DateOfBirthValidator(0, 100),
                new FavouriteNumberValidator(0, short.MaxValue),
                new FavouriteCharacterValidator(0),
                new FavouriteGameValidator(1, 60),
                new DonationsValidator(0),
            });
        }

        /// <summary>
        /// Creates custom validator.
        /// </summary>
        /// <param name="validatorBuilder">The instance of ValidatorBuilder class.</param>
        /// <returns>The instance of IRecordValidator class.</returns>
        public static IRecordValidator CreateСustom(this ValidatorBuilder validatorBuilder)
        {
            return new CompositeValidator(new List<IRecordValidator>
            {
                new FirstNameValidator(2, 15),
                new LastNameValidator(2, 15),
                new DateOfBirthValidator(18, 65),
                new FavouriteNumberValidator(0, 9),
                new FavouriteCharacterValidator(-1),
                new FavouriteGameValidator(1, 15),
                new DonationsValidator(0.5m),
            });
        }
    }
}
