using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
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
    
    public TextFileLoadResult Load(string filePath)
    {
        try
        {
            // ファイルのエンコーディングを検出
            var detection = CharsetDetector.DetectFromFile(filePath);
            if (detection is null || detection.Detected is null) throw new InvalidOperationException($"ファイルのエンコーディングを検出できませんでした: {filePath}");
            DetectionDetail encodingResult = detection.Detected;

            var fileContent = File.ReadAllText(filePath, encodingResult.Encoding);

            var result = new TextFileLoadResult(
                IsSuccess: true,
                Content: fileContent,
                Encoding: encodingResult.Encoding,
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
                FilePath: filePath
                );
            return result;
        }

    }
    public void Save(string filePath, string content)
    {

    }
    public bool Exists(string filePath) => File.Exists(filePath);

}
public record TextFileLoadResult(
    bool IsSuccess,
    string Content,
    Encoding Encoding,
    string FilePath
    );
