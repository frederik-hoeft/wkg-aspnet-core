using System.ComponentModel.DataAnnotations;
using Wkg.Data.Validation;

namespace Wkg.AspNetCore.Validation;

/// <summary>
/// Specifies that a data field value must be a valid phone number.
/// </summary>
/// <remarks>
/// This attribute uses <see cref="DataValidationService.IsPhoneNumber(string?)"/> to validate against the phone number format.
/// </remarks>
public sealed class ValidPhoneNumberAttribute() : DataTypeAttribute(DataType.PhoneNumber)
{
    /// <summary>
    /// Determines whether the specified value conforms to the phone number format.
    /// </summary>
    /// <param name="value">The value to validate.</param>
    /// <returns><see langword="true" /> if the specified value is a valid phone number or <see langword="null" />; otherwise, <see langword="false" />.</returns>
    public override bool IsValid(object? value)
    {
        if (ErrorMessage is null && ErrorMessageResourceName is null)
        {
            ErrorMessage = "The {0} field is not a valid phone number.";
        }

        if (value is null)
        {
            return true;
        }

        if (value is not string valueAsString)
        {
            return false;
        }
        return DataValidationService.IsPhoneNumber(valueAsString);
    }
}
