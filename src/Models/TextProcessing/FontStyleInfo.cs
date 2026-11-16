using System.Windows;
using System.Windows.Media;
using R3;

namespace Reoreo125.Memopad.Models.TextProcessing;

public record FontStyleInfo(string Name, FontStyle Style, FontWeight Weight)
{
    public static IEnumerable<FontStyleInfo> FromFontFamily(string fontName)
    {
        if (string.IsNullOrEmpty(fontName)) return Enumerable.Empty<FontStyleInfo>();
        if(Fonts.SystemFontFamilies.Any(f => f.Source.Equals(fontName, StringComparison.CurrentCultureIgnoreCase)) is false)
        {
            return Array.Empty<FontStyleInfo>();
        }

        var family = new FontFamily(fontName);
        var result = family.GetTypefaces()
            .Select(tf => new FontStyleInfo(
                tf.FaceNames.FirstOrDefault().Value ?? "Regular",
                tf.Style,
                tf.Weight))
            .OrderBy(x => x.Weight.ToOpenTypeWeight())
            .Distinct();
        return result;
    }

    /// <summary>
    /// フォントファミリとスタイル名から対応するスタイル情報を解決する。
    /// スタイル名が一致しない場合やフォントが存在しない場合でも例外とせず、既定のスタイルへフォールバックする。
    /// </summary>
    /// <param name="fontName">フォントファミリ名。</param>
    /// <param name="styleName">スタイル名。</param>
    public static FontStyleInfo Resolve(string fontName, string styleName)
    {
        var styles = FromFontFamily(fontName);
        return styles.FirstOrDefault(value => value.Name == styleName)
            ?? styles.FirstOrDefault(value => Defaults.FontStyleNames.Contains(value.Name))
            ?? styles.FirstOrDefault()
            ?? new FontStyleInfo("Regular", FontStyles.Normal, FontWeights.Normal);
    }
}
