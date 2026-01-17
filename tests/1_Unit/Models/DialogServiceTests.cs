using NSubstitute;
using System.Windows;
using IPrismDialogService = Prism.Dialogs.IDialogService;
using DialogService = Reoreo125.Memopad.Models.DialogService;

namespace Reoreo125.Memopad.Tests.Unit.Models;

[Collection("DisableTestParallelization")]
public class DialogServiceTests
{
    public DialogServiceTests()
    {
        if (Application.Current is null)
        {
            new Application { ShutdownMode = ShutdownMode.OnExplicitShutdown };
        }
    }

    #region ShowConfirmSave
    [Fact(DisplayName = "【正常系】ShowConfirmSave: ダイアログが表示されること")]
    public void ShowConfirmSave_ShouldShowDialog()
    {
        var prismDialogService = Substitute.For<IPrismDialogService>();

        var expectedResult = new DialogResult(ButtonResult.OK);
        prismDialogService.WhenForAnyArgs(x => x.ShowDialog(default, default, default))
            .Do(callInfo =>
            {
                var callback = callInfo.ArgAt<dynamic>(2);
                callback?.Invoke(expectedResult);
            });

        var dialogService = new DialogService
        {
            PrismDialogService = prismDialogService,
        };

        var actualResult = dialogService.ShowConfirmSave("TestText");

        prismDialogService
            .ReceivedWithAnyArgs(1)
            .ShowDialog(default, default, default);
        Assert.NotNull(actualResult);
        Assert.Equal(expectedResult, actualResult);
    }
    #endregion

    #region ShowNotFound
    [Fact(DisplayName = "【正常系】ShowNotFound: ダイアログが表示されること")]
    public void ShowNotFound_ShouldShowDialog()
    {
        var prismDialogService = Substitute.For<IPrismDialogService>();
        var expectedResult = new DialogResult(ButtonResult.OK);
        prismDialogService.WhenForAnyArgs(x => x.ShowDialog(default, default, default))
            .Do(callInfo =>
            {
                var callback = callInfo.ArgAt<dynamic>(2);
                callback?.Invoke(expectedResult);
            });

        var dialogService = new DialogService { PrismDialogService = prismDialogService };
        var actualResult = dialogService.ShowNotFound("test");

        prismDialogService.ReceivedWithAnyArgs(1).ShowDialog(default, default, default);
        Assert.NotNull(actualResult);
        Assert.Equal(expectedResult, actualResult);
    }
    #endregion

    #region ShowGoToLine
    [Fact(DisplayName = "【正常系】ShowGoToLine: ダイアログが表示されること")]
    public void ShowGoToLine_ShouldShowDialog()
    {
        var prismDialogService = Substitute.For<IPrismDialogService>();
        var expectedResult = new DialogResult(ButtonResult.OK);
        prismDialogService.WhenForAnyArgs(x => x.ShowDialog(default, default, default))
            .Do(callInfo =>
            {
                var callback = callInfo.ArgAt<dynamic>(2);
                callback?.Invoke(expectedResult);
            });

        var dialogService = new DialogService { PrismDialogService = prismDialogService };
        var actualResult = dialogService.ShowGoToLine(1);

        prismDialogService.ReceivedWithAnyArgs(1).ShowDialog(default, default, default);
        Assert.NotNull(actualResult);
        Assert.Equal(expectedResult, actualResult);
    }
    #endregion

    #region ShowFontNotFound
    [Fact(DisplayName = "【正常系】ShowFontNotFound: ダイアログが表示されること")]
    public void ShowFontNotFound_ShouldShowDialog()
    {
        var prismDialogService = Substitute.For<IPrismDialogService>();
        var expectedResult = new DialogResult(ButtonResult.OK);
        prismDialogService.WhenForAnyArgs(x => x.ShowDialog(default, default, default))
            .Do(callInfo =>
            {
                var callback = callInfo.ArgAt<dynamic>(2);
                callback?.Invoke(expectedResult);
            });

        var dialogService = new DialogService { PrismDialogService = prismDialogService };
        var actualResult = dialogService.ShowFontNotFound("test");

        prismDialogService.ReceivedWithAnyArgs(1).ShowDialog(default, default, default);
        Assert.NotNull(actualResult);
        Assert.Equal(expectedResult, actualResult);
    }
    #endregion

    #region ShowLineOutOfBounds
    [Fact(DisplayName = "【正常系】ShowLineOutOfBounds: ダイアログが表示されること")]
    public void ShowLineOutOfBounds_ShouldShowDialog()
    {
        var prismDialogService = Substitute.For<IPrismDialogService>();
        var expectedResult = new DialogResult(ButtonResult.OK);
        prismDialogService.WhenForAnyArgs(x => x.ShowDialog(default, default, default))
            .Do(callInfo =>
            {
                var callback = callInfo.ArgAt<dynamic>(2);
                callback?.Invoke(expectedResult);
            });

        var dialogService = new DialogService { PrismDialogService = prismDialogService };
        var actualResult = dialogService.ShowLineOutOfBounds();

        prismDialogService.ReceivedWithAnyArgs(1).ShowDialog(default, default, default);
        Assert.NotNull(actualResult);
        Assert.Equal(expectedResult, actualResult);
    }
    #endregion

    #region ShowAbout
    [Fact(DisplayName = "【正常系】ShowAbout: ダイアログが表示されること")]
    public void ShowAbout_ShouldShowDialog()
    {
        var prismDialogService = Substitute.For<IPrismDialogService>();
        var expectedResult = new DialogResult(ButtonResult.OK);
        prismDialogService.WhenForAnyArgs(x => x.ShowDialog(default, default, default))
            .Do(callInfo =>
            {
                var callback = callInfo.ArgAt<dynamic>(2);
                callback?.Invoke(expectedResult);
            });

        var dialogService = new DialogService { PrismDialogService = prismDialogService };
        var actualResult = dialogService.ShowAbout();

        prismDialogService.ReceivedWithAnyArgs(1).ShowDialog(default, default, default);
        Assert.NotNull(actualResult);
        Assert.Equal(expectedResult, actualResult);
    }
    #endregion

    #region ShowPageSettings
    [Fact(DisplayName = "【正常系】ShowPageSettings: ダイアログが表示されること")]
    public void ShowPageSettings_ShouldShowDialog()
    {
        var prismDialogService = Substitute.For<IPrismDialogService>();
        var expectedResult = new DialogResult(ButtonResult.OK);
        prismDialogService.WhenForAnyArgs(x => x.ShowDialog(default, default, default))
            .Do(callInfo =>
            {
                var callback = callInfo.ArgAt<dynamic>(2);
                callback?.Invoke(expectedResult);
            });

        var dialogService = new DialogService { PrismDialogService = prismDialogService };
        var actualResult = dialogService.ShowPageSettings();

        prismDialogService.ReceivedWithAnyArgs(1).ShowDialog(default, default, default);
        Assert.NotNull(actualResult);
        Assert.Equal(expectedResult, actualResult);
    }
    #endregion

    #region ShowFont
    [Fact(DisplayName = "【正常系】ShowFont: ダイアログが表示されること")]
    public void ShowFont_ShouldShowDialog()
    {
        var prismDialogService = Substitute.For<IPrismDialogService>();
        var expectedResult = new DialogResult(ButtonResult.OK);
        prismDialogService.WhenForAnyArgs(x => x.ShowDialog(default, default, default))
            .Do(callInfo =>
            {
                var callback = callInfo.ArgAt<dynamic>(2);
                callback?.Invoke(expectedResult);
            });

        var dialogService = new DialogService { PrismDialogService = prismDialogService };
        var actualResult = dialogService.ShowFont();

        prismDialogService.ReceivedWithAnyArgs(1).ShowDialog(default, default, default);
        Assert.NotNull(actualResult);
        Assert.Equal(expectedResult, actualResult);
    }
    #endregion

    #region ShowFileLoadError
    [Fact(DisplayName = "【正常系】ShowFileLoadError: ダイアログが表示されること")]
    public void ShowFileLoadError_ShouldShowDialog()
    {
        var prismDialogService = Substitute.For<IPrismDialogService>();
        var expectedResult = new DialogResult(ButtonResult.OK);
        prismDialogService.WhenForAnyArgs(x => x.ShowDialog(default, default, default))
            .Do(callInfo =>
            {
                var callback = callInfo.ArgAt<dynamic>(2);
                callback?.Invoke(expectedResult);
            });

        var dialogService = new DialogService { PrismDialogService = prismDialogService };
        var actualResult = dialogService.ShowFileLoadError();

        prismDialogService.ReceivedWithAnyArgs(1).ShowDialog(default, default, default);
        Assert.NotNull(actualResult);
        Assert.Equal(expectedResult, actualResult);
    }
    #endregion

    #region ShowFileSaveError
    [Fact(DisplayName = "【正常系】ShowFileSaveError: ダイアログが表示されること")]
    public void ShowFileSaveError_ShouldShowDialog()
    {
        var prismDialogService = Substitute.For<IPrismDialogService>();
        var expectedResult = new DialogResult(ButtonResult.OK);
        prismDialogService.WhenForAnyArgs(x => x.ShowDialog(default, default, default))
            .Do(callInfo =>
            {
                var callback = callInfo.ArgAt<dynamic>(2);
                callback?.Invoke(expectedResult);
            });

        var dialogService = new DialogService { PrismDialogService = prismDialogService };
        var actualResult = dialogService.ShowFileSaveError();

        prismDialogService.ReceivedWithAnyArgs(1).ShowDialog(default, default, default);
        Assert.NotNull(actualResult);
        Assert.Equal(expectedResult, actualResult);
    }
    #endregion
    
    #region ShowFind
    [Fact(DisplayName = "【正常系】ShowFind: ダイアログが表示されること")]
    public void ShowFind_ShouldShowDialog()
    {
        var prismDialogService = Substitute.For<IPrismDialogService>();
        var expectedResult = new DialogResult(ButtonResult.OK);
        prismDialogService.WhenForAnyArgs(x => x.Show(default, default, default))
            .Do(callInfo =>
            {
                var callback = callInfo.ArgAt<dynamic>(2);
                callback?.Invoke(expectedResult);
            });

        var dialogService = new DialogService { PrismDialogService = prismDialogService };
        var actualResult = dialogService.ShowFind();

        prismDialogService.ReceivedWithAnyArgs(1).Show(default, default, default);
        Assert.NotNull(actualResult);
        Assert.Equal(expectedResult, actualResult);
    }
    #endregion

    #region ShowReplace
    [Fact(DisplayName = "【正常系】ShowReplace: ダイアログが表示されること")]
    public void ShowReplace_ShouldShowDialog()
    {
        var prismDialogService = Substitute.For<IPrismDialogService>();
        var expectedResult = new DialogResult(ButtonResult.OK);
        prismDialogService.WhenForAnyArgs(x => x.Show(default, default, default))
            .Do(callInfo =>
            {
                var callback = callInfo.ArgAt<dynamic>(2);
                callback?.Invoke(expectedResult);
            });

        var dialogService = new DialogService { PrismDialogService = prismDialogService };
        var actualResult = dialogService.ShowReplace();

        prismDialogService.ReceivedWithAnyArgs(1).Show(default, default, default);
        Assert.NotNull(actualResult);
        Assert.Equal(expectedResult, actualResult);
    }
    #endregion
}
