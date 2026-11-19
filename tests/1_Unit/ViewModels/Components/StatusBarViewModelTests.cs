using System.Text;
using System.Windows;
using NSubstitute;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.TextProcessing;
using Reoreo125.Memopad.ViewModels.Components;

namespace Reoreo125.Memopad.Tests.Unit.ViewModels.Components;

public class StatusBarViewModelTests
{
    IEditorService EditorService { get; }
    IEditorDocument Document { get; }
    ISettingsService SettingsService { get; }
    ISettings Settings { get; }

    public StatusBarViewModelTests()
    {
        EditorService = Substitute.For<IEditorService>();
        Document = Substitute.For<IEditorDocument>();
        Document.Row.Returns(new ReactiveProperty<int>(1));
        Document.Column.Returns(new ReactiveProperty<int>(1));
        Document.LineEnding.Returns(new ReactiveProperty<LineEnding>(Defaults.LineEnding));
        Document.Encoding.Returns(new ReactiveProperty<Encoding>(Defaults.Encoding));
        Document.HasBom.Returns(new ReactiveProperty<bool>(Defaults.HasBOM));
        EditorService.Document.Returns(Document);

        SettingsService = Substitute.For<ISettingsService>();
        Settings = Substitute.For<ISettings>();
        Settings.ShowStatusBar.Returns(new ReactiveProperty<bool>(Defaults.ShowStatusBar));
        Settings.ZoomLevel.Returns(new ReactiveProperty<int>(Defaults.ZoomLevel));
        SettingsService.Settings.Returns(Settings);
    }

    #region コンストラクタ
    [Fact(DisplayName = "【異常系】コンストラクタ:EditorServiceがnullの場合、ArgumentNullExceptionを送出すること")]
    public void Constructor_WithNullEditorService_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new StatusBarViewModel(null!, SettingsService));

        Assert.Equal("editorService", exception.ParamName);
    }

    [Fact(DisplayName = "【異常系】コンストラクタ:SettingsServiceがnullの場合、ArgumentNullExceptionを送出すること")]
    public void Constructor_WithNullSettingsService_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new StatusBarViewModel(EditorService, null!));

        Assert.Equal("settingsService", exception.ParamName);
    }
    #endregion

    #region 初期値
    [Fact(DisplayName = "【正常系】初期値:各表示項目が既定値で初期化されること")]
    public void Constructor_ShouldInitializeWithDefaults()
    {
        using var viewModel = new StatusBarViewModel(EditorService, SettingsService);

        Assert.Equal(Visibility.Visible, viewModel.ShowStatusBar.Value);
        Assert.Equal(Defaults.PositionText, viewModel.PositionText.Value);
        Assert.Equal(Defaults.ZoomLevelText, viewModel.ZoomLevelText.Value);
        Assert.Equal("Windows (CRLF)", viewModel.LineEndingText.Value);
        // BOM無しの場合、BOM表記が空になるため末尾にスペースが残る
        Assert.Equal("UTF-8 ", viewModel.EncodingText.Value);
    }

    [Fact(DisplayName = "【正常系】初期値:ステータスバー非表示の設定がCollapsedとして反映されること")]
    public void Constructor_WhenShowStatusBarIsFalse_ShouldBeCollapsed()
    {
        Settings.ShowStatusBar.Returns(new ReactiveProperty<bool>(false));

        using var viewModel = new StatusBarViewModel(EditorService, SettingsService);

        Assert.Equal(Visibility.Collapsed, viewModel.ShowStatusBar.Value);
    }
    #endregion

    #region ShowStatusBar
    [Theory(DisplayName = "【正常系】ShowStatusBar:設定値の変更に追従すること")]
    [InlineData(true, Visibility.Visible)]
    [InlineData(false, Visibility.Collapsed)]
    public void ShowStatusBar_ShouldFollowSettings(bool showStatusBar, Visibility expected)
    {
        using var viewModel = new StatusBarViewModel(EditorService, SettingsService);

        Settings.ShowStatusBar.Value = showStatusBar;

        Assert.Equal(expected, viewModel.ShowStatusBar.Value);
    }
    #endregion

    #region PositionText
    [Fact(DisplayName = "【正常系】PositionText:行・列の変更に追従すること")]
    public void PositionText_ShouldFollowRowAndColumn()
    {
        using var viewModel = new StatusBarViewModel(EditorService, SettingsService);

        Document.Row.Value = 3;
        Assert.Equal("3行、1列", viewModel.PositionText.Value);

        Document.Column.Value = 12;
        Assert.Equal("3行、12列", viewModel.PositionText.Value);
    }
    #endregion

    #region ZoomLevelText
    [Theory(DisplayName = "【正常系】ZoomLevelText:ズームレベルの変更に追従すること")]
    [InlineData(10, "10%")]
    [InlineData(150, "150%")]
    [InlineData(500, "500%")]
    public void ZoomLevelText_ShouldFollowZoomLevel(int zoomLevel, string expected)
    {
        using var viewModel = new StatusBarViewModel(EditorService, SettingsService);

        Settings.ZoomLevel.Value = zoomLevel;

        Assert.Equal(expected, viewModel.ZoomLevelText.Value);
    }
    #endregion

    #region LineEndingText
    [Theory(DisplayName = "【正常系】LineEndingText:改行コードの変更に追従すること")]
    [InlineData(LineEnding.CRLF, "Windows (CRLF)")]
    [InlineData(LineEnding.LF, "Linux (LF)")]
    [InlineData(LineEnding.CR, "Macintosh (CR)")]
    [InlineData(LineEnding.Unknown, "Unknown")]
    public void LineEndingText_ShouldFollowLineEnding(LineEnding lineEnding, string expected)
    {
        using var viewModel = new StatusBarViewModel(EditorService, SettingsService);

        Document.LineEnding.Value = lineEnding;

        Assert.Equal(expected, viewModel.LineEndingText.Value);
    }
    #endregion

    #region EncodingText
    [Fact(DisplayName = "【正常系】EncodingText:文字コード・BOMの変更に追従すること")]
    public void EncodingText_ShouldFollowEncodingAndBom()
    {
        using var viewModel = new StatusBarViewModel(EditorService, SettingsService);

        Assert.Equal("UTF-8 ", viewModel.EncodingText.Value);

        Document.Encoding.Value = Encoding.Unicode;
        Assert.Equal("UTF-16 ", viewModel.EncodingText.Value);

        Document.HasBom.Value = true;
        Assert.Equal("UTF-16 (BOM 付き)", viewModel.EncodingText.Value);

        Document.HasBom.Value = false;
        Assert.Equal("UTF-16 ", viewModel.EncodingText.Value);
    }
    #endregion

    #region Dispose
    [Fact(DisplayName = "【正常系】Dispose:複数回呼び出しても例外が発生しないこと")]
    public void Dispose_CalledMultipleTimes_ShouldNotThrow()
    {
        var viewModel = new StatusBarViewModel(EditorService, SettingsService);

        viewModel.Dispose();
        viewModel.Dispose();
    }
    #endregion
}
