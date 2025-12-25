using System.Drawing;
using System.Text;
using System.Windows;
using Reoreo125.Memopad.Models.TextProcessing;

namespace Reoreo125.Memopad.Models;

public record MemopadDefaults
{
    public static string ApplicationName => "Memopad";
    public static string NewFileName => "無題";
    public static string FileExtension => ".txt";
    public static LineEnding LineEnding => LineEnding.CRLF;
    public static Encoding Encoding => Encoding.UTF8;
    public static bool HasBOM => false;
    public static string EncodingText => Encoding.WebName.ToUpper();
    public static string PositionText => "1行、1列";
    public static FontFamily TextFont => FontFamily.GenericMonospace;
    public static double ZoomLevel => 1.0;
    public static double ZoomStep => 0.1;
    public static double ZoomMax => 5.0;
    public static double ZoomMin => 0.1;
    public static string ZoomLevelText => "100%";
    public static int FontSize => 12;
    public static TextWrapping TextWrapping => TextWrapping.NoWrap;
    public static int TextBoxDebounce => 500;
}
