using NSubstitute;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Tests.TestHelpers;
using Reoreo125.Memopad.ViewModels.Dialogs;
using IDialogService = Reoreo125.Memopad.Models.IDialogService;

namespace Reoreo125.Memopad.Tests.Unit.ViewModels.Dialogs;

public class GoToLineDialogViewModelTests
{
    IEditorService EditorService { get; }
    IDialogService DialogService { get; }

    public GoToLineDialogViewModelTests()
    {
        EditorService = Substitute.For<IEditorService>();
        DialogService = Substitute.For<IDialogService>();
    }

    #region コンストラクタ
    [Fact(DisplayName = "【異常系】コンストラクタ:EditorServiceがnullの場合、ArgumentNullExceptionを送出すること")]
    public void Constructor_WithNullEditorService_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new GoToLineDialogViewModel(null!, DialogService));

        Assert.Equal("editorService", exception.ParamName);
    }

    [Fact(DisplayName = "【異常系】コンストラクタ:DialogServiceがnullの場合、ArgumentNullExceptionを送出すること")]
    public void Constructor_WithNullDialogService_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new GoToLineDialogViewModel(EditorService, null!));

        Assert.Equal("dialogService", exception.ParamName);
    }
    #endregion

    #region OnDialogOpened
    [Fact(DisplayName = "【正常系】OnDialogOpened:パラメータの行番号が反映されること")]
    public void OnDialogOpened_ShouldSetLineIndex()
    {
        var viewModel = new GoToLineDialogViewModel(EditorService, DialogService);
        var parameters = new DialogParameters
        {
            { "lineIndex", 42 }
        };

        viewModel.OnDialogOpened(parameters);

        Assert.Equal(42, viewModel.LineIndex.Value);
    }
    #endregion

    #region GoCommand
    [Fact(DisplayName = "【正常系】GoCommand:移動に成功した場合、OKでダイアログが閉じること")]
    public void GoCommand_WhenSucceeds_ShouldCloseWithOk()
    {
        EditorService.GoToLine(42).Returns(true);
        var viewModel = new GoToLineDialogViewModel(EditorService, DialogService);
        viewModel.LineIndex.Value = 42;
        IDialogResult? result = null;
        DialogCloseListenerTestHelper.AttachRequestClose(viewModel, r => result = r);

        viewModel.GoCommand.Execute();

        EditorService.Received(1).GoToLine(42);
        Assert.Equal(ButtonResult.OK, result?.Result);
        DialogService.DidNotReceiveWithAnyArgs().ShowInformation(default!, default!);
    }

    [Fact(DisplayName = "【異常系】GoCommand:移動に失敗した場合、情報を表示しダイアログを閉じないこと")]
    public void GoCommand_WhenFails_ShouldShowInformationAndNotClose()
    {
        EditorService.GoToLine(Arg.Any<int>()).Returns(false);
        var viewModel = new GoToLineDialogViewModel(EditorService, DialogService);
        viewModel.LineIndex.Value = 42;
        IDialogResult? result = null;
        DialogCloseListenerTestHelper.AttachRequestClose(viewModel, r => result = r);

        viewModel.GoCommand.Execute();

        DialogService.Received(1).ShowInformation(
            $"{Defaults.ApplicationName} - 行に移動",
            "指定した行番号は行の総数を超えています");
        Assert.Null(result);
    }
    #endregion

    #region CancelCommand
    [Fact(DisplayName = "【正常系】CancelCommand:キャンセルでダイアログが閉じること")]
    public void CancelCommand_ShouldCloseWithCancel()
    {
        var viewModel = new GoToLineDialogViewModel(EditorService, DialogService);
        IDialogResult? result = null;
        DialogCloseListenerTestHelper.AttachRequestClose(viewModel, r => result = r);

        viewModel.CancelCommand.Execute();

        Assert.Equal(ButtonResult.Cancel, result?.Result);
        EditorService.DidNotReceiveWithAnyArgs().GoToLine(default);
    }
    #endregion
}
