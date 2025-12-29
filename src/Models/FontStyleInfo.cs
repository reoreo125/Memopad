using System.Windows;
using System.Windows.Media;
using R3;

namespace Reoreo125.Memopad.Models;

public record FontStyleInfo(string Name, FontStyle Style, FontWeight Weight)
{
    public static IEnumerable<FontStyleInfo> FromFontFamily(string fontName)
    {
        if (string.IsNullOrEmpty(fontName)) return Enumerable.Empty<FontStyleInfo>();

        var family = new FontFamily(fontName);
        return family.GetTypefaces()
            .Select(tf => new FontStyleInfo(
                tf.FaceNames.Values.FirstOrDefault() ?? "Regular",
                tf.Style,
                tf.Weight))
            .OrderBy(x => x.Weight.ToOpenTypeWeight())
            .Distinct(); // 重複排除
    }
}
