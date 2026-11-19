using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Tests.TestHelpers;
using Reoreo125.Memopad.ViewModels.Dialogs;

namespace Reoreo125.Memopad.Tests.Unit.ViewModels.Dialogs;

public class AboutDialogViewModelTests
{
    [Fact(DisplayName = "【正常系】各プロパティ:アプリケーション情報が設定されること")]
    public void Properties_ShouldContainApplicationInformation()
    {
        var viewModel = new AboutDialogViewModel();

        Assert.Equal($"{Defaults.ApplicationName} のバージョン情報", viewModel.Title);
        Assert.Equal(Defaults.ApplicationName, viewModel.Message);
        Assert.Equal(Defaults.VersionText, viewModel.VersionText);
    }

    [Fact(DisplayName = "【正常系】OkCommand:OKでダイアログが閉じること")]
    public void OkCommand_ShouldCloseWithOk()
    {
        var viewModel = new AboutDialogViewModel();
        IDialogResult? result = null;
        DialogCloseListenerTestHelper.AttachRequestClose(viewModel, r => result = r);

        viewModel.OkCommand.Execute();

        Assert.Equal(ButtonResult.OK, result?.Result);
    }

    [Fact(DisplayName = "【正常系】OnDialogOpened:パラメータがなくても例外が発生しないこと")]
    public void OnDialogOpened_WithoutParameters_ShouldNotThrow()
    {
        var viewModel = new AboutDialogViewModel();

        var exception = Record.Exception(() => viewModel.OnDialogOpened(new DialogParameters()));

        Assert.Null(exception);
    }
}
