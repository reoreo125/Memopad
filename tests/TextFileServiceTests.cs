using System.Reflection;
using System.Text;
using Reoreo125.Memopad.Models.TextProcessing;

namespace Reoreo125.Memopad.Tests;

public class TextFileServiceTests
{
    [Theory(DisplayName = "【正常系】NormalizeLineEndingsToCRLF:メソッドに入力されたテキストの改行コードが全てCRLFに変換され正規化されること")]
    [InlineData("Line1\rLine2\rLine3", "Line1\r\nLine2\r\nLine3")]
    [InlineData("Line1\nLine2\nLine3", "Line1\r\nLine2\r\nLine3")]
    [InlineData("Line1\r\nLine2\r\nLine3", "Line1\r\nLine2\r\nLine3")]
    [InlineData("Line1\rLine2\nLine3\r\n", "Line1\r\nLine2\r\nLine3\r\n")]
    public void NormalizeLineEndingsToCRLF_ShouldNormalizeCorrectly(string inputText, string expectedText)
    {
        var result = TextFileService.NormalizeLineEndingsToCRLF(inputText);

        Assert.Equal(expectedText, result);
    }
    
    [Theory(DisplayName = "【正常系】GetStringFromLineEnding:LineEndingの値に応じて適切なstringを返すこと")]
    [InlineData(LineEnding.CRLF, "\r\n")]
    [InlineData(LineEnding.CR, "\r")]
    [InlineData(LineEnding.LF, "\n")]
    [InlineData(LineEnding.Unknown, "")]
    public void GetStringFromLineEnding_Test(LineEnding lineEnding, string expected)
    {
        Assert.Equal(expected, TextFileService.GetStringFromLineEnding(lineEnding));
    }

    [Theory(DisplayName = "【正常系】GetLineEndingFromString:stringの値に応じて適切なLineEndingを返すこと")]
    [InlineData("\r\n", LineEnding.CRLF)]
    [InlineData("\r", LineEnding.CR)]
    [InlineData("\n", LineEnding.LF)]
    [InlineData("", LineEnding.Unknown)]
    public void GetLineEndingFromString(string inputTeext, LineEnding expectedLineEnding)
    {
        Assert.Equal(expectedLineEnding, TextFileService.GetLineEndingFromString(inputTeext));
    }
    
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
}
