using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Reoreo125.Memopad.Models.Validators;

public class FolderPathValidatorAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => FolderPathValidator.Validate(value!, validationContext);
}
public class FolderPathValidator : IValidator
{
    public static string Name => "FolderPath";

    public static ValidationResult? Validate(object? value, ValidationContext? validationContext)
    {
        // string型でない場合
        if (value is not string forlderpath) return new ValidationResult("{0} is not a string.", [Name]);
        // 空白だった場合
        if (string.IsNullOrWhiteSpace(forlderpath)) return new ValidationResult("The {0} field is required.", [Name]);
        // 使用できない文字が含まれている場合
        if (forlderpath.Any(c => Path.GetInvalidPathChars().Contains(c))) return new ValidationResult("{0} contains characters that cannot be used.", [Name]);
        // 存在しないフォルダパスだった場合
        if (Directory.Exists(forlderpath) is false) return new ValidationResult("{0} is not a valid folder path.", [Name]);

        return ValidationResult.Success;
    }

    ValidationResult? IValidator.Validate(object? value, ValidationContext? validationContext)
        => Validate(value!, validationContext!);
    bool IValidator.Validate(object? value)
        => Validate(value!);
    public static bool Validate(object? value)
        => Validate(value!, null) == ValidationResult.Success;
}
