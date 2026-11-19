using NSubstitute;
using R3;
using Reoreo125.Memopad.Models;
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
