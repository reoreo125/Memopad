using System.Drawing;
using System.Printing;
using System.Reflection;
using System.Text;
using System.Windows;
using Newtonsoft.Json;
using R3;
using Reoreo125.Memopad.Models.TextProcessing;
using FontStyle = System.Windows.FontStyle;

namespace Reoreo125.Memopad.Models;

public record Defaults
{
    public static string ApplicationName => Assembly.GetExecutingAssembly().GetName().Name!;
    public static Version Version => Assembly.GetEntryAssembly()?.GetName().Version!;
    public static string VersionText => $"Version {Version.ToString(3)}";
    public static string NewFileName => "無題";
    public static string FileExtension => ".txt";
    public static LineEnding LineEnding => LineEnding.CRLF;
    public static Encoding Encoding => Encoding.UTF8;
    public static bool HasBOM => false;
    public static string EncodingText => Encoding.WebName.ToUpper();
    public static string PositionText => "1行、1列";
    public static string FontFamilyName => "Consolas";
    public static string FontStyleName => "Regular";
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
    public static bool MatchCase => false;
    public static bool WrapAround => false;
    public static int SearchTextMaxLength => 128;
    public static int ReplaceTextMaxLength => 128;
    public static PageMediaSizeName PaperSizeName => PageMediaSizeName.ISOA4;
    public static InputBin InputBin => InputBin.AutoSelect;
    public static PageOrientation PageOrientation => PageOrientation.Portrait;
    public static double MarginLeft => 20.0d;
    public static double MarginTop => 25.0d;
    public static double MarginRight => 20.0d;
    public static double MarginBottom => 25.0d;
    public static string Header => "&f";
    public static string Footer => "Page &p";
}
