using System;
using System.Collections.Generic;
using System.IO;
using FileCabinetApp.Validators;
using Microsoft.Extensions.Configuration;

namespace FileCabinetApp.Extensions.ValidatorExtensions
{
    /// <summary>
    /// Extends ValidatorBuilder class.
    /// </summary>
    public static class ValidatorExtension
    {
        /// <summary>
        /// Creates default validator.
        /// </summary>
        /// <param name="validatorBuilder">The instance of ValidatorBuilder class.</param>
        /// <returns>The instance of IRecordValidator class.</returns>
        public static IRecordValidator CreateDefault(this ValidatorBuilder validatorBuilder)
        {
            var rulesets = GetRulesets();

            ValidationRuleset ruleset = rulesets.Default;

            return new CompositeValidator(new List<IRecordValidator>
            {
                new FirstNameValidator(ruleset.FirstName.MinLength, ruleset.FirstName.MaxLength),
                new LastNameValidator(ruleset.LastName.MinLength, ruleset.LastName.MaxLength),
                new DateOfBirthValidator(ruleset.DateOfBirth.From, ruleset.DateOfBirth.To),
                new FavouriteNumberValidator(ruleset.FavNumber.MinValue, ruleset.FavNumber.MaxValue),
                new FavouriteCharacterValidator(ruleset.FavCharacter.SymbolCase),
                new FavouriteGameValidator(ruleset.FavGame.MinLength, ruleset.FavGame.MaxLength),
                new DonationsValidator(ruleset.Donations.MinValue),
            });
        }

        /// <summary>
        /// Creates custom validator.
        /// </summary>
        /// <param name="validatorBuilder">The instance of ValidatorBuilder class.</param>
        /// <returns>The instance of IRecordValidator class.</returns>
        public static IRecordValidator CreateСustom(this ValidatorBuilder validatorBuilder)
        {
            var rulesets = GetRulesets();

            ValidationRuleset ruleset = rulesets.Custom;

            return new CompositeValidator(new List<IRecordValidator>
            {
                new FirstNameValidator(ruleset.FirstName.MinLength, ruleset.FirstName.MaxLength),
                new LastNameValidator(ruleset.LastName.MinLength, ruleset.LastName.MaxLength),
                new DateOfBirthValidator(ruleset.DateOfBirth.From, ruleset.DateOfBirth.To),
                new FavouriteNumberValidator(ruleset.FavNumber.MinValue, ruleset.FavNumber.MaxValue),
                new FavouriteCharacterValidator(ruleset.FavCharacter.SymbolCase),
                new FavouriteGameValidator(ruleset.FavGame.MinLength, ruleset.FavGame.MaxLength),
                new DonationsValidator(ruleset.Donations.MinValue),
            });
        }

        private static AppConfiguration GetRulesets()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("validation-rules.json");

            IConfiguration config = builder.Build();

            var rulesets = ConfigurationBinder.Get<AppConfiguration>(config);

            return rulesets;
        }
    }
}
