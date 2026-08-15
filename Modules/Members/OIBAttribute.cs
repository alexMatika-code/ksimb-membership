using System.ComponentModel.DataAnnotations;

namespace ksimb_membership.Modules.Members;

public sealed class OIBAttribute : ValidationAttribute
{
    public OIBAttribute()
    {
        ErrorMessage = "OIB nije ispravan.";
    }

    protected override ValidationResult? IsValid(
        object? value,
        ValidationContext validationContext)
    {
        if (value is null)
            return ValidationResult.Success;

        var oib = value.ToString();

        if (string.IsNullOrWhiteSpace(oib))
            return ValidationResult.Success;

        if (oib.Length != 11 || !oib.All(char.IsDigit))
            return new ValidationResult(ErrorMessage);

        var remainder = 10;

        for (var i = 0; i < 10; i++)
        {
            var digit = oib[i] - '0';

            remainder += digit;
            remainder %= 10;

            if (remainder == 0)
                remainder = 10;

            remainder *= 2;
            remainder %= 11;
        }

        var controlDigit = 11 - remainder;

        if (controlDigit == 10)
            controlDigit = 0;

        var actualControlDigit = oib[10] - '0';

        return controlDigit == actualControlDigit
            ? ValidationResult.Success
            : new ValidationResult(ErrorMessage);
    }
}