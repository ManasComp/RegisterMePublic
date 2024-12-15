#region

using System.ComponentModel.DataAnnotations;

#endregion

namespace WebGui.Areas.Visitor.Models;

public class DateOnlyRangeAttribute(string minDate, string? maxDate = null) : ValidationAttribute
{
    private readonly DateOnly
        _maxDate = maxDate == null ? DateOnly.FromDateTime(DateTime.Now) : DateOnly.Parse(maxDate);

    private readonly DateOnly _minDate = DateOnly.Parse(minDate);

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not DateOnly dateValue)
        {
            return new ValidationResult("Invalid date format");
        }

        if (dateValue <= _minDate || dateValue >= _maxDate)
        {
            return new ValidationResult(ErrorMessage);
        }

        return ValidationResult.Success;
    }
}
