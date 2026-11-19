using System.Windows;
using System.Windows.Media;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.TextProcessing;

namespace Reoreo125.Memopad.Tests.Unit.Models.TextProcessing;

public class FontStyleInfoTests
{
    #region FromFontFamily
    [Fact(DisplayName = "【正常系】存在するフォントファミリー名から正しいスタイル情報を取得できること")]
    public void FromFontFamily_WithValidFont_ShouldReturnCorrectStyles()
    {
        IEnumerable<FontStyleInfo> styles = FontStyleInfo.FromFontFamily("Consolas");

        Assert.NotNull(styles);
        Assert.NotEmpty(styles);

        Assert.Contains(styles, s => s.Name == "Regular" && s.Style == FontStyles.Normal && s.Weight == FontWeights.Normal);
        Assert.Contains(styles, s => s.Name == "Bold" && s.Style == FontStyles.Normal && s.Weight == FontWeights.Bold);
    }

    [Fact(DisplayName = "【異常系】存在しないフォントファミリー名の場合に空のコレクションを返すこと")]
    public void FromFontFamily_WithInvalidFont_ShouldReturnEmpty()
    {
        IEnumerable<FontStyleInfo> styles = FontStyleInfo.FromFontFamily("NonExistentFont");

        Assert.NotNull(styles);
        Assert.Empty(styles);
    }

    [Theory(DisplayName = "【異常系】nullまたは空のフォントファミリー名の場合に空のコレクションを返すこと")]
#pragma warning disable xUnit1012
    [InlineData(null)]
#pragma warning restore xUnit1012
    [InlineData("")]
    public void FromFontFamily_WithNullOrEmptyFont_ShouldReturnEmpty(string fontName)
    {
        IEnumerable<FontStyleInfo> styles = FontStyleInfo.FromFontFamily(fontName);

        Assert.NotNull(styles);
        Assert.Empty(styles);
    }
    #endregion

    #region Resolve
    [Fact(DisplayName = "【正常系】Resolve:一致するスタイル名の場合、そのスタイルを返すこと")]
    public void Resolve_WithMatchingStyleName_ShouldReturnStyle()
    {
        var styles = FontStyleInfo.FromFontFamily("Consolas").ToArray();
        var expected = styles.First();

        var actual = FontStyleInfo.Resolve("Consolas", expected.Name);

        Assert.Equal(styles.First(style => style.Name == expected.Name), actual);
    }

    [Fact(DisplayName = "【正常系】Resolve:一致しないスタイル名の場合、既定のスタイルを返すこと")]
    public void Resolve_WithUnknownStyleName_ShouldReturnDefaultStyle()
    {
        var styles = FontStyleInfo.FromFontFamily("Consolas").ToArray();

        var actual = FontStyleInfo.Resolve("Consolas", "NonExistentStyle");

        Assert.Contains(actual, styles);
        Assert.Contains(actual.Name, Defaults.FontStyleNames);
    }

    [Fact(DisplayName = "【異常系】Resolve:存在しないフォントの場合、Regular/Normal/Normalを返すこと")]
    public void Resolve_WithNonExistentFont_ShouldReturnFallback()
    {
        var actual = FontStyleInfo.Resolve("NonExistentFont", "Bold");

        Assert.Equal(new FontStyleInfo("Regular", FontStyles.Normal, FontWeights.Normal), actual);
    }

    [Theory(DisplayName = "【異常系】Resolve:フォント名がnullまたは空の場合、フォールバックを返すこと")]
#pragma warning disable xUnit1012
    [InlineData(null)]
#pragma warning restore xUnit1012
    [InlineData("")]
    public void Resolve_WithNullOrEmptyFontName_ShouldReturnFallback(string? fontName)
    {
        var actual = FontStyleInfo.Resolve(fontName!, "Bold");

        Assert.Equal(new FontStyleInfo("Regular", FontStyles.Normal, FontWeights.Normal), actual);
    }

    [Fact(DisplayName = "【正常系】Resolve:既定のスタイル名を持たないフォントの場合、先頭のスタイルを返すこと")]
    public void Resolve_FontWithoutDefaultStyleName_ShouldReturnFirstStyle()
    {
        var target = Fonts.SystemFontFamilies
            .Select(family => new
            {
                Family = family,
                Styles = FontStyleInfo.FromFontFamily(family.Source).ToArray()
            })
            .FirstOrDefault(x =>
                x.Styles.Length > 0 &&
                x.Styles.All(style => !Defaults.FontStyleNames.Contains(style.Name)));

        if (target is null)
        {
            Assert.Skip("既定のスタイル名(Regular/Normal/Medium)を持たないフォントがインストールされていません。");
        }

        var actual = FontStyleInfo.Resolve(target.Family.Source, "NonExistentStyle");

        Assert.Equal(target.Styles[0], actual);
    }
    #endregion
}
