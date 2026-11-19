using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Tests.TestHelpers;
using Reoreo125.Memopad.ViewModels.Dialogs;

namespace Reoreo125.Memopad.Tests.Unit.ViewModels.Dialogs;

public class ConfirmSaveDialogViewModelTests
{
    [Fact(DisplayName = "【正常系】Title:アプリケーション名が表示されること")]
    public void Title_ShouldBeApplicationName()
    {
        var viewModel = new ConfirmSaveDialogViewModel();

        Assert.Equal(Defaults.ApplicationName, viewModel.Title);
    }

    [Fact(DisplayName = "【正常系】OnDialogOpened:パラメータのメッセージが反映されること")]
    public void OnDialogOpened_ShouldSetMessage()
    {
        var viewModel = new ConfirmSaveDialogViewModel();
        var parameters = new DialogParameters
        {
            { "message", "変更内容を保存しますか？" }
        };

        viewModel.OnDialogOpened(parameters);

        Assert.Equal("変更内容を保存しますか？", viewModel.Message);
    }

    [Theory(DisplayName = "【正常系】各コマンド:対応するButtonResultでダイアログが閉じること")]
    [InlineData(ButtonResult.Yes)]
    [InlineData(ButtonResult.No)]
    [InlineData(ButtonResult.Cancel)]
    public void Commands_ShouldCloseWithExpectedResult(ButtonResult expected)
    {
        var viewModel = new ConfirmSaveDialogViewModel();
        IDialogResult? result = null;
        DialogCloseListenerTestHelper.AttachRequestClose(viewModel, r => result = r);

        var command = expected switch
        {
            ButtonResult.Yes => viewModel.SaveCommand,
            ButtonResult.No => viewModel.DontSaveCommand,
            ButtonResult.Cancel => viewModel.CancelCommand,
            _ => throw new ArgumentOutOfRangeException(nameof(expected))
        };
        command.Execute();

        Assert.Equal(expected, result?.Result);
    }

    [Fact(DisplayName = "【正常系】CanCloseDialog:常にtrueを返すこと")]
    public void CanCloseDialog_ShouldReturnTrue()
    {
        var viewModel = new ConfirmSaveDialogViewModel();

        Assert.True(viewModel.CanCloseDialog());
    }
}
