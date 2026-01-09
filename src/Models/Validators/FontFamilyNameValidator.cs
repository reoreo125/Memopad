using System.ComponentModel.DataAnnotations;
using System.Windows.Media;

namespace Reoreo125.Memopad.Models.Validators;

public class FontFamilyNameValidatorAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => FontFamilyNameValidator.Validate(value!, validationContext);
}
public class FontFamilyNameValidator : IValidator
{
    public static string Name => "FontFamilyName";

    public static ValidationResult? Validate(object? value, ValidationContext? validationContext)
    {
        // string型でない場合
        if (value is not string fontFamilyName) return new ValidationResult("{0} is not a string.", [Name]);
        // 空白だった場合
        if (string.IsNullOrWhiteSpace(fontFamilyName)) return new ValidationResult("The {0} field is required.", [Name]);
        // システムにインストールされているフォント名の中に存在するかチェック
        // SourceName または現在のカルチャでの名前（FamilyNames）で判定
        if( Fonts.SystemFontFamilies.Any(f =>
            f.Source == fontFamilyName ||
            f.FamilyNames.Values.Contains(fontFamilyName)) is false)
        {
            return new ValidationResult("The font '{0}' is not available.", [Name]);
        }

        return ValidationResult.Success;
    }

    ValidationResult? IValidator.Validate(object? value, ValidationContext? validationContext)
        => Validate(value!, validationContext!);
    bool IValidator.Validate(object? value)
        => Validate(value!);
    public static bool Validate(object? value)
        => Validate(value!, null) == ValidationResult.Success;
}
