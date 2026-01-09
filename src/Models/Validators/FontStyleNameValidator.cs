using System.ComponentModel.DataAnnotations;
using System.Windows.Media;
using R3;

namespace Reoreo125.Memopad.Models.Validators;

public class FontStyleNameValidatorAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext? validationContext)
        => FontStyleNameValidator.Validate(value!, validationContext);
}
public class FontStyleNameValidator : IValidator
{
    public static string Name => "FontStyleName";

    public static ValidationResult? Validate(object? value, ValidationContext? validationContext)
    {
        // validationContextがnullだった場合
        if (validationContext is null) return new ValidationResult("{0} requires a ValidationContext.", [Name]);
        // string型でない場合
        if (value is not string fontStyleName) return new ValidationResult("{0} is not a string.", [Name]);
        // 空白だった場合
        if (string.IsNullOrWhiteSpace(fontStyleName)) return new ValidationResult("The {0} field is required.", [Name]);

        // 1. 同じオブジェクト(Settings)から FontFamilyName プロパティを探す
        var familyProp = validationContext.ObjectType.GetProperty("FontFamilyName");
        if (familyProp is null) return new ValidationResult("{0} requires a FontFamilyName", [Name]);

        // 2. 現在の FontFamilyName の値（ReactiveProperty<string>）を取得
        var familyRx = familyProp.GetValue(validationContext.ObjectInstance) as ReactiveProperty<string>;
        var familyName = familyRx?.Value;

        if (string.IsNullOrEmpty(familyName)) return new ValidationResult("FontFamilyName is empty.");

        // 3. そのフォントに指定されたスタイルが存在するかチェック
        var family = Fonts.SystemFontFamilies.FirstOrDefault(f =>
            f.Source == familyName || f.FamilyNames.Values.Contains(familyName));

        if (family == null) return new ValidationResult("Font family not found.");

        // Typefaceの一覧から、一致するスタイル(Normal, Italicなど)があるか確認
        bool hasStyle = family.GetTypefaces().Any(t =>
            t.Style.ToString() == fontStyleName ||
            t.FaceNames.Values.Contains(fontStyleName));

        if (hasStyle is false)
        {
            return new ValidationResult("The FontStyle '{0}' is not available.", [Name]);
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
