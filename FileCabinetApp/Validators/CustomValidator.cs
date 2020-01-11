using System;
using System.Collections.Generic;

namespace FileCabinetApp.Validators
{
    /// <summary>
    /// Custom validator for user's input.
    /// </summary>
    public class CustomValidator : CompositeValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomValidator"/> class.
        /// </summary>
        public CustomValidator()
            : base(new IRecordValidator[]
            {
                new FirstNameValidator(2, 15),
                new LastNameValidator(2, 15),
                new DateOfBirthValidator(18, 65),
                new FavouriteNumberValidator(0, 10),
                new FavouriteCharacterValidator(-1),
                new FavouriteGameValidator(2, 15),
                new DonationsValidator(0.5m),
            })
        {
        }
    }
}
