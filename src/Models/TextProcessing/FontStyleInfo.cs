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
}
