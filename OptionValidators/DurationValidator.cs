using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Validation;

namespace HttpBomb.OptionValidators
{
    internal class DurationValidator : IOptionValidator
    {
        public ValidationResult GetValidationResult(CommandOption option, ValidationContext context)
        {
            if (option.HasValue() && int.TryParse(option.Value(), out var time) && time >= 1 && time <= 3600)
                return ValidationResult.Success;
            else
                return new ValidationResult($"The value for -{option.ShortName}|--{option.LongName} is required and must be between 1 and 3600 seconds.");
        }
    }
}