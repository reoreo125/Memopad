using System.Text;

namespace Reoreo125.Memopad.Models.TextProcessing;

public record TextFileLoadResult(
    bool IsSuccess,
    string FilePath,

    string Content,
    Encoding Encoding,
    bool HasBOM,
    LineEnding LineEnding
    );
