using System.Reflection;
using System.Text;
using Reoreo125.Memopad.Models.TextProcessing;

namespace Reoreo125.Memopad.Tests;

public class TextFileServiceTests
{
    #region NormalizeLineEndingsToCRLF
    [Theory(DisplayName = "【正常系】NormalizeLineEndingsToCRLF:メソッドに入力されたテキストの改行コードが全てCRLFに変換され正規化されること")]
    [InlineData("Line1\rLine2\rLine3", "Line1\r\nLine2\r\nLine3")]
    [InlineData("Line1\nLine2\nLine3", "Line1\r\nLine2\r\nLine3")]
    [InlineData("Line1\r\nLine2\r\nLine3", "Line1\r\nLine2\r\nLine3")]
    [InlineData("Line1\rLine2\nLine3\r\n", "Line1\r\nLine2\r\nLine3\r\n")]
    [InlineData("Line1 Line2 Line3", "Line1 Line2 Line3")]
    public void NormalizeLineEndingsToCRLF_ShouldNormalizeCorrectly(string inputText, string expectedText)
    {
        var result = TextFileService.NormalizeLineEndingsToCRLF(inputText);

        Assert.Equal(expectedText, result);
    }
    #endregion

    #region GetStringFromLineEnding
    [Theory(DisplayName = "【正常系】GetStringFromLineEnding:LineEndingの値に応じて適切なstringを返すこと")]
    [InlineData(LineEnding.CRLF, "\r\n")]
    [InlineData(LineEnding.CR, "\r")]
    [InlineData(LineEnding.LF, "\n")]
    [InlineData(LineEnding.Unknown, "")]
    public void GetStringFromLineEnding_ValidValue_Test(LineEnding lineEnding, string expected)
    {
        Assert.Equal(expected, TextFileService.GetStringFromLineEnding(lineEnding));
    }
    [Theory(DisplayName = "【異常系】GetStringFromLineEnding:LineEndingの不正な値に対して空文字を返すこと")]
    [InlineData((LineEnding)48693, "")]
    [InlineData((LineEnding)99999, "")]
    public void GetStringFromLineEnding_InvalidValue_Test(LineEnding lineEnding, string expected)
    {
        Assert.Equal(expected, TextFileService.GetStringFromLineEnding(lineEnding));
    }
    #endregion

    #region GetLineEndingFromString
    [Theory(DisplayName = "【正常系】GetLineEndingFromString:stringの値に応じて適切なLineEndingを返すこと")]
    [InlineData("\r\n", LineEnding.CRLF)]
    [InlineData("\r", LineEnding.CR)]
    [InlineData("\n", LineEnding.LF)]
    [InlineData("", LineEnding.Unknown)]
    public void GetLineEndingFromString_ValidValue_Test(string inputText, LineEnding expectedLineEnding)
    {
        Assert.Equal(expectedLineEnding, TextFileService.GetLineEndingFromString(inputText));
    }
    [Theory(DisplayName = "【異常系】GetLineEndingFromString:stringの不正な値に対してLineEnding.Unknownを返すこと")]
    [InlineData("aaaa", LineEnding.Unknown)]
    [InlineData("test", LineEnding.Unknown)]
    [InlineData("r", LineEnding.Unknown)]
    public void GetLineEndingFromString_InvalidValue_Test(string inputText, LineEnding expectedLineEnding)
    {
        Assert.Equal(expectedLineEnding, TextFileService.GetLineEndingFromString(inputText));
    }
    #endregion

    #region Save
    [Fact(DisplayName = "【正常系】Save:TextFileServiceを経由して保存しファイルが生成され中身が一致すること")]
    public void Save_ShouldCreateFile()
    {
        var service = new TextFileService();
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_save_{Guid.NewGuid()}.txt");
        var content = "Hello, World!";

        try
        {
            var result = service.Save(tempPath, content, Encoding.UTF8, false, LineEnding.CRLF);
            
            Assert.True(result.IsSuccess);
            Assert.True(File.Exists(tempPath));
            Assert.Equal(content, File.ReadAllText(tempPath));
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }
    #endregion
}
