using System.IO;
using System.Text;
using UtfUnknown;

namespace Reoreo125.Memopad.Models.Services;

public interface ITextFileService
{
    TextFileLoadResult Load(string filePath);
    void Save(string filePath, string content);
    bool Exists(string filePath);
}

public class TextFileService : ITextFileService
{
    public MemopadSettings LoadSettings()
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;

        var settings = new MemopadSettings
        {

        };
        return null;
    }
    public TextFileLoadResult Load(string filePath)
    {
        try
        {
            // ファイルのエンコーディングを検出
            var detection = CharsetDetector.DetectFromFile(filePath);
            if (detection is null || detection.Detected is null) throw new InvalidOperationException($"ファイルのエンコーディングを検出できませんでした: {filePath}");
            DetectionDetail encodingResult = detection.Detected;

            // 改行コードのチェック
            LineEnding lineEnding = LineEnding.Unknown;
            using (var reader = new StreamReader(filePath, encodingResult.Encoding))
            {
                int c;
                while ((c = reader.Read()) != -1)
                {
                    if (c == '\r')
                    {
                        if (reader.Peek() == '\n')
                        {
                            lineEnding = LineEnding.CRLF; // CRLF (Windows)
                            break;
                        }
                        lineEnding = LineEnding.CR; // CR (古いMac)
                        break;
                    }
                    if (c == '\n')
                    {
                        lineEnding = LineEnding.LF; // LF (Unix/Linux/macOS)
                        break;
                    }
                }
            }

            var fileContent = File.ReadAllText(filePath, encodingResult.Encoding);

            var result = new TextFileLoadResult(
                IsSuccess: true,
                Content: fileContent,
                Encoding: encodingResult.Encoding,
                LineEnding: lineEnding,
                FilePath: filePath
                );
            return result;
        }
        catch
        {
            var result = new TextFileLoadResult(
                IsSuccess: false,
                Content: string.Empty,
                Encoding: Encoding.Default,
                LineEnding: LineEnding.Unknown,
                FilePath: filePath
                );
            return result;
        }

    }
    public void Save(string filePath, string content)
    {

    }
    public bool Exists(string filePath) => File.Exists(filePath);

    public static LineEnding GetLineEndingFromString(string lineEnding) => lineEnding switch
    {
        "\r\n" => LineEnding.CRLF,
        "\n" => LineEnding.LF,
        "\r" => LineEnding.CR,
        _ => LineEnding.Unknown
    };

}
public record TextFileLoadResult(
    bool IsSuccess,
    string Content,
    Encoding Encoding,
    LineEnding LineEnding,
    string FilePath
    );
public enum LineEnding
{
    Unknown,
    CRLF, // Windows (\r\n)
    LF,   // Unix/macOS (\n)
    CR    // Old Mac (\r)
}
