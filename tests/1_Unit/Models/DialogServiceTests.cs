using NSubstitute;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Views.Dialogs;
using System.Windows;
using IPrismDialogService = Prism.Dialogs.IDialogService;
using PrismDialogWindow = Prism.Dialogs.DialogWindow;
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

    #region ShowInformation / ShowWarning / ShowError
    private static void AssertShowsMessageDialog(
        string expectedDialogName,
        string title,
        string message,
        Func<DialogService, IDialogResult?> show)
    {
        var prismDialogService = Substitute.For<IPrismDialogService>();
        var expectedResult = new DialogResult(ButtonResult.OK);
        string? actualDialogName = null;
        IDialogParameters? actualParameters = null;
        var callCount = 0;

        prismDialogService.WhenForAnyArgs(x => x.ShowDialog(default, default, default))
            .Do(callInfo =>
            {
                callCount++;
                actualDialogName = callInfo.ArgAt<string>(0);
                actualParameters = callInfo.ArgAt<IDialogParameters>(1);
                var callback = callInfo.ArgAt<dynamic>(2);
                callback?.Invoke(expectedResult);
            });

        var dialogService = new DialogService { PrismDialogService = prismDialogService };
        var actualResult = show(dialogService);

        Assert.Equal(1, callCount);
        Assert.Equal(expectedDialogName, actualDialogName);
        Assert.NotNull(actualParameters);
        Assert.Equal(title, actualParameters.GetValue<string>("title"));
        Assert.Equal(message, actualParameters.GetValue<string>("message"));
        Assert.Equal(expectedResult, actualResult);
    }

    [Fact(DisplayName = "【正常系】ShowInformation: 情報ダイアログがタイトル・メッセージ付きで表示されること")]
    public void ShowInformation_ShouldShowInformationDialogWithParameters()
    {
        AssertShowsMessageDialog(
            nameof(InformationDialog),
            "タイトル",
            "メッセージ",
            service => service.ShowInformation("タイトル", "メッセージ"));
    }

    [Fact(DisplayName = "【正常系】ShowWarning: 警告ダイアログがタイトル・メッセージ付きで表示されること")]
    public void ShowWarning_ShouldShowWarningDialogWithParameters()
    {
        AssertShowsMessageDialog(
            nameof(WarningDialog),
            "警告",
            "警告メッセージ",
            service => service.ShowWarning("警告", "警告メッセージ"));
    }

    [Fact(DisplayName = "【正常系】ShowError: エダイアログがタイトル・メッセージ付きで表示されること")]
    public void ShowError_ShouldShowErrorDialogWithParameters()
    {
        AssertShowsMessageDialog(
            nameof(ErrorDialog),
            "エラー",
            "エラーメッセージ",
            service => service.ShowError("エラー", "エラーメッセージ"));
    }

    [Fact(DisplayName = "【異常系】ShowInformation: コールバックが呼ばれない場合、nullを返すこと")]
    public void ShowInformation_WhenCallbackNotInvoked_ShouldReturnNull()
    {
        var prismDialogService = Substitute.For<IPrismDialogService>();
        string? actualDialogName = null;
        IDialogParameters? actualParameters = null;
        var callCount = 0;
        prismDialogService.WhenForAnyArgs(x => x.ShowDialog(default, default, default))
            .Do(callInfo =>
            {
                callCount++;
                actualDialogName = callInfo.ArgAt<string>(0);
                actualParameters = callInfo.ArgAt<IDialogParameters>(1);
            });

        var dialogService = new DialogService { PrismDialogService = prismDialogService };
        var actualResult = dialogService.ShowInformation("タイトル", "メッセージ");

        Assert.Null(actualResult);
        Assert.Equal(1, callCount);
        Assert.Equal(nameof(InformationDialog), actualDialogName);
        Assert.NotNull(actualParameters);
        Assert.Equal("タイトル", actualParameters.GetValue<string>("title"));
        Assert.Equal("メッセージ", actualParameters.GetValue<string>("message"));
    }
    #endregion

    #region ShowFind / ShowReplace（モードレス）
    private static class DialogContents
    {
        public sealed class FindDialog { }
        public sealed class ReplaceDialog { }
    }

    [StaFact(DisplayName = "【正常系】ShowFind: 同じダイアログが開いている場合、新しいダイアログを表示せず既存を前面化すること")]
    public void ShowFind_WhenSameDialogAlreadyOpen_ShouldActivateExistingWindow()
    {
        var existingWindow = new PrismDialogWindow { Content = new DialogContents.FindDialog() };
        existingWindow.WindowState = WindowState.Minimized;
        try
        {
            var prismDialogService = Substitute.For<IPrismDialogService>();
            var dialogService = new DialogService
            {
                PrismDialogService = prismDialogService,
                WindowProvider = () => new Window[] { existingWindow }
            };

            var actualResult = dialogService.ShowFind();

            Assert.Null(actualResult);
            Assert.Equal(WindowState.Normal, existingWindow.WindowState);
            prismDialogService.DidNotReceiveWithAnyArgs().Show(default, default, default);
        }
        finally
        {
            existingWindow.Close();
        }
    }

    [StaFact(DisplayName = "【正常系】ShowReplace: 別のモードレスダイアログが開いている場合、閉じてから表示すること")]
    public void ShowReplace_WhenOtherDialogOpen_ShouldCloseOtherAndShowReplace()
    {
        var otherWindow = new PrismDialogWindow { Content = new DialogContents.FindDialog() };
        var otherWindowClosed = false;
        otherWindow.Closed += (_, _) => otherWindowClosed = true;
        try
        {
            var prismDialogService = Substitute.For<IPrismDialogService>();
            var expectedResult = new DialogResult(ButtonResult.OK);
            string? shownDialogName = null;
            var showCount = 0;
            prismDialogService.WhenForAnyArgs(x => x.Show(default, default, default))
                .Do(callInfo =>
                {
                    showCount++;
                    shownDialogName = callInfo.ArgAt<string>(0);
                    var callback = callInfo.ArgAt<dynamic>(2);
                    callback?.Invoke(expectedResult);
                });

            var dialogService = new DialogService
            {
                PrismDialogService = prismDialogService,
                WindowProvider = () => new Window[] { otherWindow }
            };

            var actualResult = dialogService.ShowReplace();

            Assert.Equal(1, showCount);
            Assert.Equal(nameof(DialogContents.ReplaceDialog), shownDialogName);
            Assert.Same(expectedResult, actualResult);
            Assert.True(otherWindowClosed);
        }
        finally
        {
            if (otherWindowClosed is false) otherWindow.Close();
        }
    }

    [StaFact(DisplayName = "【正常系】ShowFind: Prismダイアログ以外のウィンドウは閉じずに新しいダイアログを表示すること")]
    public void ShowFind_WhenNonPrismWindowOpen_ShouldNotCloseIt()
    {
        var otherWindow = new Window { Content = new DialogContents.FindDialog() };
        var otherWindowClosed = false;
        otherWindow.Closed += (_, _) => otherWindowClosed = true;
        try
        {
            var prismDialogService = Substitute.For<IPrismDialogService>();
            var dialogService = new DialogService
            {
                PrismDialogService = prismDialogService,
                WindowProvider = () => new Window[] { otherWindow }
            };

            var actualResult = dialogService.ShowFind();

            Assert.Null(actualResult);
            Assert.False(otherWindowClosed);
            prismDialogService.ReceivedWithAnyArgs(1).Show(default, default, default);
        }
        finally
        {
            if (otherWindowClosed is false) otherWindow.Close();
        }
    }
    #endregion

    #region GetLastOpenedFolderPath
    [Fact(DisplayName = "【正常系】GetLastOpenedFolderPath:設定されたフォルダパスを返すこと")]
    public void GetLastOpenedFolderPath_WithSetting_ShouldReturnSetting()
    {
        var settingsService = Substitute.For<ISettingsService>();
        var settings = Substitute.For<ISettings>();
        settings.LastOpenedFolderPath.Returns(new ReactiveProperty<string>(@"C:\Temp"));
        settingsService.Settings.Returns(settings);

        var dialogService = new DialogService { SettingsService = settingsService };

        var actual = dialogService.GetLastOpenedFolderPath();

        Assert.Equal(@"C:\Temp", actual);
    }

    [Theory(DisplayName = "【正常系】GetLastOpenedFolderPath:設定が空の場合、既定のフォルダを返すこと")]
    [InlineData("")]
    [InlineData(null)]
    public void GetLastOpenedFolderPath_WhenEmpty_ShouldReturnDefault(string? folderPath)
    {
        var settingsService = Substitute.For<ISettingsService>();
        var settings = Substitute.For<ISettings>();
        settings.LastOpenedFolderPath.Returns(new ReactiveProperty<string>(folderPath!));
        settingsService.Settings.Returns(settings);

        var dialogService = new DialogService { SettingsService = settingsService };

        var actual = dialogService.GetLastOpenedFolderPath();

        Assert.Equal(Defaults.LastOpenedFolderPath, actual);
    }

    [Fact(DisplayName = "【異常系】GetLastOpenedFolderPath:SettingsService未注入の場合、InvalidOperationExceptionを送出すること")]
    public void GetLastOpenedFolderPath_WithoutSettingsService_ShouldThrow()
    {
        var dialogService = new DialogService();

        var exception = Assert.Throws<InvalidOperationException>(() => dialogService.GetLastOpenedFolderPath());

        Assert.Contains("SettingsService", exception.Message);
    }
    #endregion
}
