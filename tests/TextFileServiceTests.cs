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
    [Fact(DisplayName = "【正常系】Save:ファイルが生成され中身が一致すること")]
    public void Save_ShouldCreateFile()
    {
        var service = new TextFileService();
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_save_{Guid.NewGuid()}.txt");
        var content = "Test Content";

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

    [Fact(DisplayName = "【正常系】Save:UTF-8(BOM有り)でファイルが保存されること")]
    public void Save_ShouldSaveFile_UTF8_WithBOM()
    {
        var service = new TextFileService();
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_save_{Guid.NewGuid()}.txt");
        var content = "Test Content";

        try
        {
            var result = service.Save(tempPath, content, Encoding.UTF8, true, LineEnding.CRLF);
            
            Assert.True(result.IsSuccess);
            var fileBytes = File.ReadAllBytes(tempPath);
            var preamble = Encoding.UTF8.GetPreamble();
            Assert.True(preamble.SequenceEqual(fileBytes.Take(preamble.Length)));
            Assert.Equal(content, File.ReadAllText(tempPath, Encoding.UTF8));
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }
    
    [Fact(DisplayName = "【正常系】Save:Shift-JISでファイルが保存されること")]
    public void Save_ShouldSaveFile_ShiftJIS()
    {
        var service = new TextFileService();
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_save_{Guid.NewGuid()}.txt");
        var content = "テストコンテンツ";
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        var sjis = Encoding.GetEncoding("Shift_JIS");

        try
        {
            var result = service.Save(tempPath, content, sjis, false, LineEnding.CRLF);
            
            Assert.True(result.IsSuccess);
            Assert.Equal(content, File.ReadAllText(tempPath, sjis));
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    [Fact(DisplayName = "【正常系】Save:LF改行でファイルが保存されること")]
    public void Save_ShouldSaveFile_With_LF()
    {
        var service = new TextFileService();
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_save_{Guid.NewGuid()}.txt");
        var content = "Line1\nLine2";

        try
        {
            var result = service.Save(tempPath, content, Encoding.UTF8, false, LineEnding.LF);
            
            Assert.True(result.IsSuccess);
            var fileContent = File.ReadAllText(tempPath);
            Assert.Contains("\n", fileContent);
            Assert.DoesNotContain("\r\n", fileContent);
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    [Fact(DisplayName = "【正常系】Save:CR改行でファイルが保存されること")]
    public void Save_ShouldSaveFile_With_CR()
    {
        var service = new TextFileService();
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_save_{Guid.NewGuid()}.txt");
        var content = "Line1\rLine2";

        try
        {
            var result = service.Save(tempPath, content, Encoding.UTF8, false, LineEnding.CR);
            
            Assert.True(result.IsSuccess);
            var fileContent = File.ReadAllText(tempPath);
            Assert.Contains("\r", fileContent);
            Assert.DoesNotContain("\r\n", fileContent);
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    [Fact(DisplayName = "【正常系】Save:既存のファイルを上書きできること")]
    public void Save_ShouldOverwriteFile()
    {
        var service = new TextFileService();
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_save_{Guid.NewGuid()}.txt");
        var initialContent = "Initial Content";
        var newContent = "New Content";
        File.WriteAllText(tempPath, initialContent);

        try
        {
            var result = service.Save(tempPath, newContent, Encoding.UTF8, false, LineEnding.CRLF);
            
            Assert.True(result.IsSuccess);
            Assert.Equal(newContent, File.ReadAllText(tempPath));
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    [Fact(DisplayName = "【異常系】Save:不正なパスに保存しようとするとIsSuccessがfalseになること")]
    public void Save_ShouldFailWithInvalidPath()
    {
        var service = new TextFileService();
        var invalidPath = Path.Combine(":", "invalid_file.txt");
        var content = "Test Content";

        var result = service.Save(invalidPath, content, Encoding.UTF8, false, LineEnding.CRLF);
        
        Assert.False(result.IsSuccess);
    }
    #endregion

    #region Load
    [Fact(DisplayName = "【正常系】Load:UTF-8(BOM無し)のファイルが読み込めること")]
    public void Load_ShouldReadFile_UTF8_NoBOM()
    {
        var service = new TextFileService();
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_load_{Guid.NewGuid()}.txt");
        var content = "Test Content";
        File.WriteAllText(tempPath, content, new UTF8Encoding(false));

        try
        {
            var result = service.Load(tempPath);

            Assert.True(result.IsSuccess);
            Assert.Equal(content, result.Content);
            Assert.Equal(Encoding.UTF8, result.Encoding);
            Assert.False(result.HasBOM);
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    [Fact(DisplayName = "【正常系】Load:UTF-8(BOM有り)のファイルが読み込めること")]
    public void Load_ShouldReadFile_UTF8_WithBOM()
    {
        var service = new TextFileService();
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_load_{Guid.NewGuid()}.txt");
        var content = "Test Content";
        File.WriteAllText(tempPath, content, new UTF8Encoding(true));

        try
        {
            var result = service.Load(tempPath);

            Assert.True(result.IsSuccess);
            Assert.Equal(content, result.Content);
            Assert.Equal(Encoding.UTF8, result.Encoding);
            Assert.True(result.HasBOM);
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    [Fact(DisplayName = "【正常系】Load:Shift-JISのファイルが読み込めること")]
    public void Load_ShouldReadFile_ShiftJIS()
    {
        var service = new TextFileService();
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_load_{Guid.NewGuid()}.txt");
        var content = "テストコンテンツ";
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        File.WriteAllText(tempPath, content, Encoding.GetEncoding("Shift_JIS"));

        try
        {
            var result = service.Load(tempPath);

            Assert.True(result.IsSuccess);
            Assert.Equal(content, result.Content);
            Assert.Equal(Encoding.GetEncoding("Shift_JIS"), result.Encoding);
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    [Fact(DisplayName = "【正常系】Load:改行コードが混在していてもCRLFに正規化されること")]
    public void Load_ShouldNormalizeLineEndings()
    {
        var service = new TextFileService();
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_load_{Guid.NewGuid()}.txt");
        var content = "Line1\rLine2\nLine3\r\n";
        var expectedContent = "Line1\r\nLine2\r\nLine3\r\n";
        File.WriteAllText(tempPath, content, new UTF8Encoding(false));

        try
        {
            var result = service.Load(tempPath);

            Assert.True(result.IsSuccess);
            Assert.Equal(expectedContent, result.Content);
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    [Fact(DisplayName = "【正常系】Load:空のファイルが読み込めること")]
    public void Load_ShouldReadEmptyFile()
    {
        var service = new TextFileService();
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_load_{Guid.NewGuid()}.txt");
        File.WriteAllText(tempPath, string.Empty);

        try
        {
            var result = service.Load(tempPath);

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Content);
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    [Fact(DisplayName = "【異常系】Load:ファイルが存在しない場合にIsSuccessがfalseになること")]
    public void Load_ShouldFailIfFileDoesNotExist()
    {
        var service = new TextFileService();
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_load_{Guid.NewGuid()}.txt");
        
        var result = service.Load(tempPath);

        Assert.False(result.IsSuccess);
    }
    #endregion

    #region DetectLineEnding
    [Fact(DisplayName = "【正常系】DetectLineEnding:CRLF改行コードを正しく検出できること")]
    public void DetectLineEnding_ShouldDetectCRLF()
    {
        var service = new TextFileService();
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_line_ending_{Guid.NewGuid()}.txt");
        File.WriteAllText(tempPath, "Line1\r\nLine2", Encoding.UTF8);

        try
        {
            var result = TextFileService.DetectLineEnding(tempPath, Encoding.UTF8);
            Assert.Equal(LineEnding.CRLF, result);
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    [Fact(DisplayName = "【正常系】DetectLineEnding:LF改行コードを正しく検出できること")]
    public void DetectLineEnding_ShouldDetectLF()
    {
        var service = new TextFileService();
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_line_ending_{Guid.NewGuid()}.txt");
        File.WriteAllText(tempPath, "Line1\nLine2", Encoding.UTF8);

        try
        {
            var result = TextFileService.DetectLineEnding(tempPath, Encoding.UTF8);
            Assert.Equal(LineEnding.LF, result);
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    [Fact(DisplayName = "【正常系】DetectLineEnding:CR改行コードを正しく検出できること")]
    public void DetectLineEnding_ShouldDetectCR()
    {
        var service = new TextFileService();
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_line_ending_{Guid.NewGuid()}.txt");
        File.WriteAllText(tempPath, "Line1\rLine2", Encoding.UTF8);

        try
        {
            var result = TextFileService.DetectLineEnding(tempPath, Encoding.UTF8);
            Assert.Equal(LineEnding.CR, result);
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    [Fact(DisplayName = "【正常系】DetectLineEnding:改行コードがない場合にUnknownを検出すること")]
    public void DetectLineEnding_ShouldDetectUnknown_NoLineEnding()
    {
        var service = new TextFileService();
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_line_ending_{Guid.NewGuid()}.txt");
        File.WriteAllText(tempPath, "No line endings", Encoding.UTF8);

        try
        {
            var result = TextFileService.DetectLineEnding(tempPath, Encoding.UTF8);
            Assert.Equal(LineEnding.Unknown, result);
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    [Fact(DisplayName = "【正常系】DetectLineEnding:空ファイルの場合にUnknownを検出すること")]
    public void DetectLineEnding_ShouldDetectUnknown_EmptyFile()
    {
        var service = new TextFileService();
        var tempPath = Path.Combine(Path.GetTempPath(), $"test_line_ending_{Guid.NewGuid()}.txt");
        File.WriteAllText(tempPath, string.Empty, Encoding.UTF8);

        try
        {
            var result = TextFileService.DetectLineEnding(tempPath, Encoding.UTF8);
            Assert.Equal(LineEnding.Unknown, result);
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
