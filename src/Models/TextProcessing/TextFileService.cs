using System.IO;
using System.Text;
using UtfUnknown;

namespace Reoreo125.Memopad.Models.TextProcessing;

public interface ITextFileService
{
    TextFileLoadResult Load(string filePath);
    TextFileSaveResult Save(string filepath, string text, Encoding encoding, bool hasBom, LineEnding lineEnding);
}

public class TextFileService : ITextFileService
{
    public TextFileLoadResult Load(string filePath)
    {
        var info = new FileInfo(filePath);

        var failedResult = new TextFileLoadResult
        (
            IsSuccess: false,
            Content: string.Empty,
            Encoding: Encoding.Default,
            HasBOM: false,
            LineEnding: LineEnding.Unknown,
            FilePath: filePath
        );

        // ファイルが存在しない
        if (!info.Exists)
        {
            return failedResult;
        }

        // ファイルが空
        if (info.Length == 0)
        {
            var emptyResult = new TextFileLoadResult
            (
                IsSuccess: true,
                Content: string.Empty,
                Encoding: Defaults.Encoding,
                HasBOM: Defaults.HasBOM,
                LineEnding: Defaults.LineEnding,
                FilePath: filePath
            );
            return emptyResult;
        }

        // ファイルの内容は1回だけ読み込み、エンコーディング検出・本文デコードに同じバイト列を使用する
        var bytes = File.ReadAllBytes(filePath);

        // ファイルのエンコーディングを検出
        var detection = CharsetDetector.DetectFromBytes(bytes);
        DetectionDetail encodingResult = detection.Detected;

        // バイナリっぽかったら読み込むだけにする
        if (detection.Details.Count == 0 && detection.Detected is null)
        {
            var binaryResult = new TextFileLoadResult
            (
                IsSuccess: true,
                Content: DecodeText(bytes, Defaults.Encoding),
                Encoding: Defaults.Encoding,
                HasBOM: Defaults.HasBOM,
                LineEnding: Defaults.LineEnding,
                FilePath: filePath
            );
            return binaryResult;
        }

        // US-ASCIIはUTF-8とする。
        if (detection.Detected.Encoding == Encoding.ASCII)
        {
            detection.Detected.Encoding = Encoding.UTF8;
        }

        try
        {
            var text = DecodeText(bytes, encodingResult.Encoding);
            var lineEnding = DetectLineEnding(text);
            var normalized = NormalizeLineEndingsToCRLF(text);

            var successResult = new TextFileLoadResult(
                IsSuccess: true,
                Content: normalized,
                Encoding: encodingResult.Encoding,
                HasBOM: encodingResult.HasBOM,
                LineEnding: lineEnding,
                FilePath: filePath
                );
            return successResult;
        }
        catch
        {
            return failedResult;
        }

    }

    /// <summary>
    /// バイト列を指定エンコーディングのテキストとしてデコードする。
    /// 先頭にエンコーディングのプリアンブル（BOM）がある場合はStreamReaderと同様に取り除く。
    /// </summary>
    private static string DecodeText(byte[] bytes, Encoding encoding)
    {
        var preamble = encoding.GetPreamble();
        var offset = preamble.Length > 0 && bytes.AsSpan().StartsWith(preamble) ? preamble.Length : 0;
        return encoding.GetString(bytes, offset, bytes.Length - offset);
    }

    internal static LineEnding DetectLineEnding(string filePath, Encoding encoding)
        => DetectLineEnding(File.ReadAllText(filePath, encoding));

    internal static LineEnding DetectLineEnding(string text)
    {
        // 改行コードの検出
        for (int i = 0; i < text.Length; i++)
        {
            if (text[i] == '\r')
            {
                if (i + 1 < text.Length && text[i + 1] == '\n')
                {
                    return LineEnding.CRLF; // CRLF (Windows)
                }
                return LineEnding.CR; // CR (古いMac)
            }
            if (text[i] == '\n')
            {
                return LineEnding.LF; // LF (Unix/Linux/macOS)
            }
        }
        return LineEnding.Unknown;
    }
    internal static string NormalizeLineEndingsToCRLF(string text)
    {
        // 内部用に改行コードを CRLF に統一する処理
        var sb = new StringBuilder();
        sb.Append(text);
        // 混在している可能性も考慮し、一旦すべて \n (LF) に統一
        sb.Replace("\r\n", "\n");
        sb.Replace("\r", "\n");
        // すべての \n を \r\n (CRLF) に変換
        // これにより、LFのみ、CRのみ、混在ファイルがすべて CRLF に統一される
        sb.Replace("\n", "\r\n");

        return sb.ToString();
    }
    public TextFileSaveResult Save(string filepath, string text, Encoding encoding, bool hasBom, LineEnding lineEnding)
    {
        // BOM付き対応
        Encoding encodingForSave;
        // UTF-8 の場合は指定に従い再生成
        if (encoding is UTF8Encoding)
        {
            encodingForSave = new UTF8Encoding(hasBom);
        }
        // UTF-16 (Unicode) の場合も同様の制御が必要
        else if (encoding is UnicodeEncoding)
        {
            encodingForSave = new UnicodeEncoding(bigEndian: false, byteOrderMark: hasBom);
        }
        // UTF-32
        else if (encoding is UTF32Encoding)
        {
            encodingForSave = new UTF32Encoding(bigEndian: false, byteOrderMark: hasBom);
        }
        else
        {
            encodingForSave = encoding; // Shift-JIS などBOMのないものはそのまま
        }

        try
        {
            using (var writer = new StreamWriter(filepath, false, encodingForSave))
            {
                var lineEndingString = GetStringFromLineEnding(lineEnding);

                writer.NewLine = lineEndingString;

                var sb = new StringBuilder();
                sb.Append(text);

                sb.Replace("\r\n", "\n");
                sb.Replace("\r", "\n");

                if (lineEndingString != "\n")
                {
                    sb.Replace("\n", lineEndingString);
                }

                foreach(var chunk in sb.GetChunks())
                {
                    writer.Write(chunk.Span);
                }
            }
            return new TextFileSaveResult(
                IsSuccess: true,
                FilePath: filepath
                );
        }
        catch
        {
            return new TextFileSaveResult(
                IsSuccess: false,
                FilePath: filepath
                );
        }
    }

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

public record TextFileSaveResult(
    bool IsSuccess,
    string FilePath
);

public record TextFileLoadResult(
    bool IsSuccess,
    string FilePath,

    string Content,
    Encoding Encoding,
    bool HasBOM,
    LineEnding LineEnding
);
