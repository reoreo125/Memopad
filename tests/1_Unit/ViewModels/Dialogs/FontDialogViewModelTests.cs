using System.Windows.Media;
using NSubstitute;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.TextProcessing;
using Reoreo125.Memopad.Tests.TestHelpers;
using Reoreo125.Memopad.ViewModels.Dialogs;
using IDialogService = Reoreo125.Memopad.Models.IDialogService;

namespace Reoreo125.Memopad.Tests.Unit.ViewModels.Dialogs;

public class FontDialogViewModelTests
{
    IEditorService EditorService { get; }
    ISettingsService SettingsService { get; }
    ISettings Settings { get; }
    IDialogService DialogService { get; }

    private readonly string _fontName;
    private readonly FontStyleInfo _fontStyle;

    public FontDialogViewModelTests()
    {
        _fontName = Fonts.SystemFontFamilies
            .Select(family => family.Source)
            .OrderBy(name => name)
            .First();
        _fontStyle = FontStyleInfo.FromFontFamily(_fontName).First();

        EditorService = Substitute.For<IEditorService>();
        DialogService = Substitute.For<IDialogService>();

        SettingsService = Substitute.For<ISettingsService>();
        Settings = Substitute.For<ISettings>();
        Settings.FontFamilyName.Returns(new ReactiveProperty<string>(_fontName));
        Settings.FontStyleName.Returns(new ReactiveProperty<string>(_fontStyle.Name));
        Settings.FontSize.Returns(new ReactiveProperty<int>(Defaults.FontSize));
        SettingsService.Settings.Returns(Settings);
    }

    private FontDialogViewModel CreateViewModel() => new(EditorService, SettingsService, DialogService);

    #region コンストラクタ
    [Fact(DisplayName = "【異常系】コンストラクタ:EditorServiceがnullの場合、ArgumentNullExceptionを送出すること")]
    public void Constructor_WithNullEditorService_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new FontDialogViewModel(null!, SettingsService, DialogService));

        Assert.Equal("editorService", exception.ParamName);
    }

    [Fact(DisplayName = "【異常系】コンストラクタ:SettingsServiceがnullの場合、ArgumentNullExceptionを送出すること")]
    public void Constructor_WithNullSettingsService_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new FontDialogViewModel(EditorService, null!, DialogService));

        Assert.Equal("settingsService", exception.ParamName);
    }

    [Fact(DisplayName = "【異常系】コンストラクタ:DialogServiceがnullの場合、ArgumentNullExceptionを送出すること")]
    public void Constructor_WithNullDialogService_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new FontDialogViewModel(EditorService, SettingsService, null!));

        Assert.Equal("dialogService", exception.ParamName);
    }
    #endregion

    #region 初期値
    [Fact(DisplayName = "【正常系】初期値:設定とフォント情報が反映されること")]
    public void Constructor_ShouldInitializeFromSettingsAndFonts()
    {
        using var viewModel = CreateViewModel();

        Assert.Equal(_fontName, viewModel.FontName.Value);
        Assert.Contains(_fontName, viewModel.FontNames);
        Assert.Equal(_fontStyle.Name, viewModel.FontStyleInfo.Value?.Name);
        Assert.Contains(viewModel.FontStyles, style => style.Name == _fontStyle.Name);
        Assert.Equal(_fontName, viewModel.ListBoxFontName.Value);
        Assert.Equal(Defaults.FontSize.ToString(), viewModel.Size.Value);
        Assert.Equal(Defaults.FontSize.ToString(), viewModel.ListBoxSize.Value);
        Assert.NotEmpty(viewModel.AvailableCharacterSets);
        Assert.False(string.IsNullOrEmpty(viewModel.SampleText.Value));
    }
    #endregion

    #region フォント名・スタイル
    [Fact(DisplayName = "【正常系】ListBoxFontName:選択したフォント名がFontNameに反映されること")]
    public void ListBoxFontName_ShouldUpdateFontNameAndStyles()
    {
        var otherFontName = Fonts.SystemFontFamilies
            .Select(family => family.Source)
            .FirstOrDefault(name => name != _fontName);

        if (otherFontName is null)
        {
            Assert.Skip("フォントが1種類しかインストールされていません。");
        }

        using var viewModel = CreateViewModel();

        viewModel.ListBoxFontName.Value = otherFontName;

        Assert.Equal(otherFontName, viewModel.FontName.Value);
        Assert.NotEmpty(viewModel.FontStyles);
    }

    [Fact(DisplayName = "【正常系】ListBoxFontStyleInfo:選択したスタイルがFontStyleInfoに反映されること")]
    public void ListBoxFontStyleInfo_ShouldUpdateFontStyleInfo()
    {
        using var viewModel = CreateViewModel();
        var expected = viewModel.FontStyles.Last();

        viewModel.ListBoxFontStyleInfo.Value = expected;

        Assert.Equal(expected, viewModel.FontStyleInfo.Value);
    }
    #endregion

    #region サイズ
    [Fact(DisplayName = "【正常系】Size:プリセット外の値ではリスト選択が解除されること")]
    public void Size_NonPresetValue_ShouldClearListBoxSelection()
    {
        using var viewModel = CreateViewModel();

        viewModel.Size.Value = "20";
        Assert.Equal("20", viewModel.ListBoxSize.Value);

        viewModel.Size.Value = "13";
        Assert.Null(viewModel.ListBoxSize.Value);
    }

    [Fact(DisplayName = "【正常系】Size:0以下の値が1に補正されること")]
    public void Size_NonPositiveValue_ShouldBeCorrectedToMinimum()
    {
        using var viewModel = CreateViewModel();

        viewModel.Size.Value = "0";

        Assert.Equal("1", viewModel.Size.Value);
    }

    [Fact(DisplayName = "【正常系】ListBoxSize:選択したサイズがSizeに反映されること")]
    public void ListBoxSize_ShouldUpdateSize()
    {
        using var viewModel = CreateViewModel();

        viewModel.ListBoxSize.Value = "24";

        Assert.Equal("24", viewModel.Size.Value);
    }
    #endregion

    #region OkCommand
    [Fact(DisplayName = "【正常系】OkCommand:有効な設定の場合、設定を保存してOKで閉じること")]
    public void OkCommand_WithValidSettings_ShouldSaveAndClose()
    {
        using var viewModel = CreateViewModel();
        viewModel.Size.Value = "20";
        IDialogResult? result = null;
        DialogCloseListenerTestHelper.AttachRequestClose(viewModel, r => result = r);

        viewModel.OkCommand.Execute();

        Assert.Equal(ButtonResult.OK, result?.Result);
        Assert.Equal(_fontName, Settings.FontFamilyName.Value);
        Assert.Equal(_fontStyle.Name, Settings.FontStyleName.Value);
        Assert.Equal(20, Settings.FontSize.Value);
    }

    [Fact(DisplayName = "【異常系】OkCommand:存在しないフォント名の場合、情報を表示して閉じないこと")]
    public void OkCommand_WithInvalidFontName_ShouldShowInformationAndNotClose()
    {
        using var viewModel = CreateViewModel();
        viewModel.FontName.Value = "___NonExistentFont___";
        IDialogResult? result = null;
        DialogCloseListenerTestHelper.AttachRequestClose(viewModel, r => result = r);

        viewModel.OkCommand.Execute();

        Assert.Null(result);
        DialogService.Received(1).ShowInformation("フォント", Arg.Is<string>(message => message.Contains("フォントはありません")));
        Assert.Equal(_fontName, Settings.FontFamilyName.Value);
    }

    [Fact(DisplayName = "【異常系】OkCommand:スタイル未選択の場合、情報を表示して閉じないこと")]
    public void OkCommand_WithNullStyle_ShouldShowInformationAndNotClose()
    {
        using var viewModel = CreateViewModel();
        viewModel.FontStyleInfo.Value = null;
        IDialogResult? result = null;
        DialogCloseListenerTestHelper.AttachRequestClose(viewModel, r => result = r);

        viewModel.OkCommand.Execute();

        Assert.Null(result);
        DialogService.Received(1).ShowInformation("フォント", Arg.Is<string>(message => message.Contains("スタイル")));
        Assert.Equal(_fontName, Settings.FontFamilyName.Value);
    }

    [Fact(DisplayName = "【正常系】CancelCommand:キャンセルでダイアログが閉じること")]
    public void CancelCommand_ShouldCloseWithCancel()
    {
        using var viewModel = CreateViewModel();
        IDialogResult? result = null;
        DialogCloseListenerTestHelper.AttachRequestClose(viewModel, r => result = r);

        viewModel.CancelCommand.Execute();

        Assert.Equal(ButtonResult.Cancel, result?.Result);
        Assert.Equal(_fontName, Settings.FontFamilyName.Value);
    }
    #endregion

    #region Dispose
    [Fact(DisplayName = "【正常系】Dispose:破棄後にフォント名を変更してもスタイル一覧が更新されないこと")]
    public void Dispose_ShouldStopFontStyleSubscription()
    {
        var viewModel = CreateViewModel();
        var styleCount = viewModel.FontStyles.Count;

        viewModel.Dispose();
        viewModel.FontName.Value = "___NonExistentFont___";

        Assert.Equal(styleCount, viewModel.FontStyles.Count);
    }
    #endregion
}
