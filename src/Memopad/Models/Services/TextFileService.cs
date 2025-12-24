using System.IO;
using System.Text;
using UtfUnknown;

namespace Reoreo125.Memopad.Models.Services;

public interface ITextFileService
{
    TextFileLoadResult Load(string filePath);
    TextFileSaveResult Save(IMemopadCoreService memopadCoreService);
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

            // 内部用に改行コードを CRLF に統一する処理
            var sb = new StringBuilder();
            sb.Append(fileContent);
            // 混在している可能性も考慮し、一旦すべて \n (LF) に統一
            sb.Replace("\r\n", "\n");
            sb.Replace("\r", "\n");
            // すべての \n を \r\n (CRLF) に変換
            // これにより、LFのみ、CRのみ、混在ファイルがすべて CRLF に統一される
            sb.Replace("\n", "\r\n");

            var result = new TextFileLoadResult(
                IsSuccess: true,
                Content: sb.ToString(),
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
    public TextFileSaveResult Save(IMemopadCoreService memopadCoreService)
    {
        try
        {
            using (var writer = new StreamWriter(memopadCoreService.FilePath.Value, false, memopadCoreService.Encoding.Value))
            {
                var lineEnding = GetStringFromLineEnding(memopadCoreService.LineEnding.Value);

                writer.NewLine = lineEnding;

                var sb = new StringBuilder();
                sb.Append(memopadCoreService.Text.Value);

                sb.Replace("\r\n", "\n");
                sb.Replace("\r", "\n");

                if (lineEnding != "\n")
                {
                    sb.Replace("\n", lineEnding);
                }

                foreach(var chunk in sb.GetChunks())
                {
                    writer.Write(chunk.Span);
                }
            }
            return new TextFileSaveResult(
                IsSuccess: true,
                FilePath: memopadCoreService.FilePath.Value
                );
        }
        catch
        {
            return new TextFileSaveResult(
                IsSuccess: false,
                FilePath: memopadCoreService.FilePath.Value
                );
        }
    }
    public bool Exists(string filePath) => File.Exists(filePath);

    public static LineEnding GetLineEndingFromString(string lineEnding) => lineEnding switch
    {
        "\r\n" => LineEnding.CRLF,
        "\n" => LineEnding.LF,
        "\r" => LineEnding.CR,
        _ => LineEnding.Unknown
    };
    public static string GetStringFromLineEnding(LineEnding lineEnding) => lineEnding switch
    {
        LineEnding.CRLF => "\r\n",
        LineEnding.LF => "\n",
        LineEnding.CR => "\r",
        _ => string.Empty
    };

}
public record TextFileLoadResult(
    bool IsSuccess,
    string Content,
    Encoding Encoding,
    LineEnding LineEnding,
    string FilePath
    );
public record TextFileSaveResult(
    bool IsSuccess,
    string FilePath
    );
public enum LineEnding
{
    Unknown,
    CRLF, // Windows (\r\n)
    LF,   // Unix/macOS (\n)
    CR    // Old Mac (\r)
}
