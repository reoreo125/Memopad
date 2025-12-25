using System.Drawing;

namespace Reoreo125.Memopad.Models;

public record MemopadSettings
{
    public string LastOpenedFolder { get; set; } = string.Empty;
    public FontFamily TextFont { get; set; } = FontFamily.GenericMonospace;
}
