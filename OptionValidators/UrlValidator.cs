using System;
using System.ComponentModel.DataAnnotations;
using McMaster.Extensions.CommandLineUtils;
using McMaster.Extensions.CommandLineUtils.Validation;

namespace HttpBomb.OptionValidators
{
    public class UrlValidator: IOptionValidator
    {
        public static bool CheckURLValid(string str)
        {
            return Uri.TryCreate(str, UriKind.Absolute, out var uriResult) 
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
        }

        public ValidationResult GetValidationResult(CommandOption option, ValidationContext context)
        {
            if (option.HasValue() && CheckURLValid(option.Value()))
                return ValidationResult.Success;
            else
                return new ValidationResult($"Value for -{option.ShortName}|--{option.LongName} is required and must be a valid url.");
        }
    }
    
}