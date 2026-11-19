using System.Printing;
using NSubstitute;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Tests.TestHelpers;
using Reoreo125.Memopad.ViewModels.Dialogs;
using IDialogService = Reoreo125.Memopad.Models.IDialogService;

namespace Reoreo125.Memopad.Tests.Unit.ViewModels.Dialogs;

public class PageSettingsDialogViewModelTests
{
    IEditorService EditorService { get; }
    ISettingsService SettingsService { get; }
    ISettings Settings { get; }
    IPageSettings Page { get; }
    IDialogService DialogService { get; }

    public PageSettingsDialogViewModelTests()
    {
        EditorService = Substitute.For<IEditorService>();
        DialogService = Substitute.For<IDialogService>();

        SettingsService = Substitute.For<ISettingsService>();
        Settings = Substitute.For<ISettings>();
        Page = Substitute.For<IPageSettings>();
        Page.PaperSizeName.Returns(new ReactiveProperty<PageMediaSizeName>(PageMediaSizeName.ISOA4));
        Page.InputBin.Returns(new ReactiveProperty<InputBin>(InputBin.AutoSelect));
        Page.Orientation.Returns(new ReactiveProperty<PageOrientation>(PageOrientation.Portrait));
        Page.MarginLeft.Returns(new ReactiveProperty<double>(10));
        Page.MarginTop.Returns(new ReactiveProperty<double>(20));
        Page.MarginRight.Returns(new ReactiveProperty<double>(30));
        Page.MarginBottom.Returns(new ReactiveProperty<double>(40));
        Page.Header.Returns(new ReactiveProperty<string>("Header"));
        Page.Footer.Returns(new ReactiveProperty<string>("Footer"));
        Settings.Page.Returns(Page);
        SettingsService.Settings.Returns(Settings);
    }

    private static bool IsDefaultPrinterAvailable()
    {
        try
        {
            _ = LocalPrintServer.GetDefaultPrintQueue();
            return true;
        }
        catch
        {
            return false;
        }
    }

    private PageSettingsDialogViewModel CreateViewModelOrSkip()
    {
        if (IsDefaultPrinterAvailable() is false)
        {
            Assert.Skip("既定のプリンターが利用できないため、ページ設定ダイアログのテストをスキップします。");
        }

        return new PageSettingsDialogViewModel(EditorService, SettingsService, DialogService);
    }

    #region コンストラクタ
    [Fact(DisplayName = "【異常系】コンストラクタ:EditorServiceがnullの場合、ArgumentNullExceptionを送出すること")]
    public void Constructor_WithNullEditorService_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new PageSettingsDialogViewModel(null!, SettingsService, DialogService));

        Assert.Equal("editorService", exception.ParamName);
    }

    [Fact(DisplayName = "【異常系】コンストラクタ:SettingsServiceがnullの場合、ArgumentNullExceptionを送出すること")]
    public void Constructor_WithNullSettingsService_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new PageSettingsDialogViewModel(EditorService, null!, DialogService));

        Assert.Equal("settingsService", exception.ParamName);
    }

    [Fact(DisplayName = "【異常系】コンストラクタ:DialogServiceがnullの場合、ArgumentNullExceptionを送出すること")]
    public void Constructor_WithNullDialogService_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new PageSettingsDialogViewModel(EditorService, SettingsService, null!));

        Assert.Equal("dialogService", exception.ParamName);
    }
    #endregion

    #region 初期値
    [Fact(DisplayName = "【正常系】初期値:ページ設定が反映されること")]
    public void Constructor_ShouldInitializeFromSettings()
    {
        using var viewModel = CreateViewModelOrSkip();

        Assert.Equal(PageMediaSizeName.ISOA4, viewModel.PageSizeName.Value);
        Assert.Equal(PageOrientation.Portrait, viewModel.Orientation.Value);
        Assert.Equal(10, viewModel.MarginLeft.Value);
        Assert.Equal(20, viewModel.MarginTop.Value);
        Assert.Equal(30, viewModel.MarginRight.Value);
        Assert.Equal(40, viewModel.MarginBottom.Value);
        Assert.Equal("Header", viewModel.Header.Value);
        Assert.Equal("Footer", viewModel.Footer.Value);
    }
    #endregion

    #region Orientation
    [Fact(DisplayName = "【正常系】Orientation:向きの変更に応じて余白が入れ替わること")]
    public void Orientation_WhenChanged_ShouldSwapMargins()
    {
        using var viewModel = CreateViewModelOrSkip();

        viewModel.Orientation.Value = PageOrientation.Landscape;

        Assert.Equal(20, viewModel.MarginLeft.Value);
        Assert.Equal(10, viewModel.MarginTop.Value);
        Assert.Equal(40, viewModel.MarginRight.Value);
        Assert.Equal(30, viewModel.MarginBottom.Value);

        // 元に戻すと再度入れ替わる
        viewModel.Orientation.Value = PageOrientation.Portrait;

        Assert.Equal(10, viewModel.MarginLeft.Value);
        Assert.Equal(20, viewModel.MarginTop.Value);
        Assert.Equal(30, viewModel.MarginRight.Value);
        Assert.Equal(40, viewModel.MarginBottom.Value);
    }
    #endregion

    #region OkCommand
    [Fact(DisplayName = "【正常系】OkCommand:有効な設定の場合、設定を保存してOKで閉じること")]
    public void OkCommand_WithValidSettings_ShouldSaveAndClose()
    {
        using var viewModel = CreateViewModelOrSkip();
        viewModel.PageSizeName.Value = PageMediaSizeName.ISOA3;
        viewModel.Orientation.Value = PageOrientation.Landscape;
        viewModel.MarginLeft.Value = 11;
        viewModel.MarginTop.Value = 12;
        viewModel.MarginRight.Value = 13;
        viewModel.MarginBottom.Value = 14;
        viewModel.Header.Value = "New Header";
        viewModel.Footer.Value = "New Footer";
        IDialogResult? result = null;
        DialogCloseListenerTestHelper.AttachRequestClose(viewModel, r => result = r);

        viewModel.OkCommand.Execute();

        Assert.Equal(ButtonResult.OK, result?.Result);
        Assert.Equal(PageMediaSizeName.ISOA3, Page.PaperSizeName.Value);
        Assert.Equal(PageOrientation.Landscape, Page.Orientation.Value);
        Assert.Equal(11, Page.MarginLeft.Value);
        Assert.Equal(12, Page.MarginTop.Value);
        Assert.Equal(13, Page.MarginRight.Value);
        Assert.Equal(14, Page.MarginBottom.Value);
        Assert.Equal("New Header", Page.Header.Value);
        Assert.Equal("New Footer", Page.Footer.Value);
    }

    [Fact(DisplayName = "【正常系】OkCommand:余白が上限値ちょうどの場合、保存できること")]
    public void OkCommand_WithMarginAtMax_ShouldSaveAndClose()
    {
        using var viewModel = CreateViewModelOrSkip();
        viewModel.MarginLeft.Value = Defaults.MarginMax;
        viewModel.MarginTop.Value = Defaults.MarginMax;
        viewModel.MarginRight.Value = Defaults.MarginMax;
        viewModel.MarginBottom.Value = Defaults.MarginMax;
        IDialogResult? result = null;
        DialogCloseListenerTestHelper.AttachRequestClose(viewModel, r => result = r);

        viewModel.OkCommand.Execute();

        Assert.Equal(ButtonResult.OK, result?.Result);
        Assert.Equal(Defaults.MarginMax, Page.MarginLeft.Value);
        DialogService.DidNotReceiveWithAnyArgs().ShowWarning(default!, default!);
    }

    [Fact(DisplayName = "【異常系】OkCommand:余白が上限を超える場合、警告を表示して閉じないこと")]
    public void OkCommand_WithMarginOverMax_ShouldShowWarningAndNotClose()
    {
        using var viewModel = CreateViewModelOrSkip();
        viewModel.PageSizeName.Value = PageMediaSizeName.ISOA3;
        viewModel.MarginLeft.Value = Defaults.MarginMax + 1;
        IDialogResult? result = null;
        DialogCloseListenerTestHelper.AttachRequestClose(viewModel, r => result = r);

        viewModel.OkCommand.Execute();

        Assert.Null(result);
        DialogService.Received(1).ShowWarning("ページ設定", Arg.Is<string>(message => message.Contains("余白")));
        Assert.Equal(PageMediaSizeName.ISOA4, Page.PaperSizeName.Value);
    }

    [Fact(DisplayName = "【正常系】CancelCommand:キャンセルでダイアログが閉じること")]
    public void CancelCommand_ShouldCloseWithCancel()
    {
        using var viewModel = CreateViewModelOrSkip();
        IDialogResult? result = null;
        DialogCloseListenerTestHelper.AttachRequestClose(viewModel, r => result = r);

        viewModel.CancelCommand.Execute();

        Assert.Equal(ButtonResult.Cancel, result?.Result);
        Assert.Equal(PageMediaSizeName.ISOA4, Page.PaperSizeName.Value);
    }
    #endregion
}
