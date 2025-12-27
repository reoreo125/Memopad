namespace Reoreo125.Memopad.Models.TextProcessing;

public enum LineEnding
{
    Unknown,
    CRLF, // Windows (\r\n)
    LF,   // Unix/macOS (\n)
    CR    // Old Mac (\r)
}
