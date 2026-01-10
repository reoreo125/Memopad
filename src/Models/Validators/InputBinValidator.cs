using System.ComponentModel.DataAnnotations;
using System.Printing;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace Reoreo125.Memopad.Models.Validators;

public class InputBinValidatorAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => InputBinValidator.Validate(value!, validationContext);
}
public class InputBinValidator : IValidator
{
    public static string Name => "InputBin";

    public static ValidationResult? Validate(object? value, ValidationContext? validationContext)
    {
        // 型のチェック（またはnullチェック）
        if (value is not InputBin inputBin) return new ValidationResult("{0} is not a valid format.", [Name]);
        // 列挙型の定義内にあるかチェック
        if (Enum.IsDefined(typeof(InputBin), inputBin) is false) return new ValidationResult("The value of {0} is out of range.", [Name]);

        return ValidationResult.Success;
    }

    ValidationResult? IValidator.Validate(object? value, ValidationContext? validationContext)
        => Validate(value!, validationContext!);
    bool IValidator.Validate(object? value)
        => Validate(value!);
    public static bool Validate(object? value)
        => Validate(value!, null) == ValidationResult.Success;
}
