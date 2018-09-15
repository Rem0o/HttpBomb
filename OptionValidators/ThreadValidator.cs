using System;
using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Validation;

namespace HttpBomb.OptionValidators
{
    public class ThreadValidator: IOptionValidator
    {
        public ValidationResult GetValidationResult(CommandOption option, ValidationContext context)
        {
            if (option.HasValue() && int.TryParse(option.Value(), out var n) && n > 0)
                return ValidationResult.Success;
            else
                return new ValidationResult($"Value for -{option.ShortName}|--{option.LongName} is required and must be > 0.");
        }
    }
    
}