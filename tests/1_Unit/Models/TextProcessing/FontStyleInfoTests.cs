using System.Windows;
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
}
