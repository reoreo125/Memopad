using System.Text;
using System.Windows;
using Reoreo125.Memopad.Models.TextProcessing;

namespace Reoreo125.Memopad.Models;

public record Defaults
{
    public static string ApplicationName => "Memopad";
    public static string NewFileName => "無題";
    public static string FileExtension => ".txt";
    public static LineEnding LineEnding => LineEnding.CRLF;
    public static Encoding Encoding => Encoding.UTF8;
    public static bool HasBOM => false;
    public static string EncodingText => Encoding.WebName.ToUpper();
    public static string PositionText => "1行、1列";
    public static string FontFamilyName => "Consolas";
    public static int FontSize => 12;
    public static double ZoomLevel => 1.0;
    public static double ZoomStep => 0.1;
    public static double ZoomMax => 5.0;
    public static double ZoomMin => 0.1;
    public static string ZoomLevelText => "100%";
    public static TextWrapping TextWrapping => TextWrapping.NoWrap;
    public static bool IsWrapping => false;
    public static int TextBoxDebounce => 500;
    public static string LastOpenedFolderPath => Environment.GetFolderPath(Environment.SpecialFolder.Personal);
    public static int SettingsSaveInterval => 5000;
    public static bool ShowStatusBar => true;
}
