using System;
using System.Collections.Generic;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Default validator for user's input.
    /// </summary>
    public class DefaultValidator : CompositeValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultValidator"/> class.
        /// </summary>
        public DefaultValidator()
            : base(new IRecordValidator[]
            {
                new FirstNameValidator(2, 60),
                new LastNameValidator(2, 60),
                new DateOfBirthValidator(0, 100),
                new FavouriteNumberValidator(0, short.MaxValue),
                new FavouriteCharacterValidator(0),
                new FavouriteGameValidator(2, 60),
                new DonationsValidator(0),
            })
        {
        }
    }
}
