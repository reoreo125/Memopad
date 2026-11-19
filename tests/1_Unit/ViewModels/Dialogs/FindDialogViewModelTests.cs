using NSubstitute;
using R3;
using Reoreo125.Memopad.Models;
using Reoreo125.Memopad.Models.Commands;
using Reoreo125.Memopad.Tests.TestHelpers;
using Reoreo125.Memopad.ViewModels.Dialogs;

namespace Reoreo125.Memopad.Tests.Unit.ViewModels.Dialogs;

public class FindDialogViewModelTests
{
    IEditorService EditorService { get; }
    IEditorDocument Document { get; }
    ISettingsService SettingsService { get; }

    public FindDialogViewModelTests()
    {
        EditorService = Substitute.For<IEditorService>();
        Document = Substitute.For<IEditorDocument>();
        Document.SearchText.Returns(new ReactiveProperty<string>(string.Empty));
        Document.SelectedText.Returns(new ReactiveProperty<string>(string.Empty));
        Document.MatchCase.Returns(new ReactiveProperty<bool>(Defaults.MatchCase));
        Document.WrapAround.Returns(new ReactiveProperty<bool>(Defaults.WrapAround));
        EditorService.Document.Returns(Document);

        SettingsService = Substitute.For<ISettingsService>();
    }

    #region コンストラクタ
    [Fact(DisplayName = "【異常系】コンストラクタ:EditorServiceがnullの場合、ArgumentNullExceptionを送出すること")]
    public void Constructor_WithNullEditorService_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new FindDialogViewModel(null!, SettingsService));

        Assert.Equal("editorService", exception.ParamName);
    }

    [Fact(DisplayName = "【異常系】コンストラクタ:SettingsServiceがnullの場合、ArgumentNullExceptionを送出すること")]
    public void Constructor_WithNullSettingsService_ShouldThrow()
    {
        var exception = Assert.Throws<ArgumentNullException>(
            () => new FindDialogViewModel(EditorService, null!));

        Assert.Equal("settingsService", exception.ParamName);
    }
    #endregion

    #region 初期値
    [Fact(DisplayName = "【正常系】初期値:Documentの検索条件が反映されること")]
    public void Constructor_ShouldReflectDocumentValues()
    {
        Document.SearchText.Returns(new ReactiveProperty<string>("initial"));
        Document.MatchCase.Returns(new ReactiveProperty<bool>(true));
        Document.WrapAround.Returns(new ReactiveProperty<bool>(true));

        using var viewModel = new FindDialogViewModel(EditorService, SettingsService);

        Assert.Equal("initial", viewModel.SearchText.Value);
        Assert.True(viewModel.MatchCase.Value);
        Assert.True(viewModel.WrapAround.Value);
        Assert.False(viewModel.IsSearchUp.Value);
        Assert.True(viewModel.FindNextButton_IsEnabled.Value);
    }

    [Fact(DisplayName = "【正常系】FindNextButton_IsEnabled:検索文字列が空の場合に無効になること")]
    public void FindNextButton_IsEnabled_ShouldFollowSearchText()
    {
        using var viewModel = new FindDialogViewModel(EditorService, SettingsService);

        Assert.False(viewModel.FindNextButton_IsEnabled.Value);

        viewModel.SearchText.Value = "keyword";
        Assert.True(viewModel.FindNextButton_IsEnabled.Value);

        viewModel.SearchText.Value = string.Empty;
        Assert.False(viewModel.FindNextButton_IsEnabled.Value);
    }
    #endregion

    #region View -> Model
    [Fact(DisplayName = "【正常系】検索条件:ViewModelの変更がDocumentに反映されること")]
    public void ViewModelChanges_ShouldPropagateToDocument()
    {
        using var viewModel = new FindDialogViewModel(EditorService, SettingsService);

        viewModel.SearchText.Value = "keyword";
        viewModel.MatchCase.Value = true;
        viewModel.WrapAround.Value = true;

        Assert.Equal("keyword", Document.SearchText.Value);
        Assert.True(Document.MatchCase.Value);
        Assert.True(Document.WrapAround.Value);
    }
    #endregion

    #region FindCommand
    [Fact(DisplayName = "【正常系】FindCommand:検索方向に応じたコマンドが実行されること")]
    public void FindCommand_ShouldExecuteCommandAccordingToDirection()
    {
        var findNextCommand = Substitute.For<IFindNextCommand>();
        var findPrevCommand = Substitute.For<IFindPrevCommand>();
        using var viewModel = new FindDialogViewModel(EditorService, SettingsService)
        {
            FindNextCommand = findNextCommand,
            FindPrevCommand = findPrevCommand
        };

        viewModel.FindCommand.Execute();
        findNextCommand.Received(1).Execute(Arg.Any<object?>());
        findPrevCommand.DidNotReceiveWithAnyArgs().Execute(default);

        viewModel.IsSearchUp.Value = true;
        viewModel.FindCommand.Execute();
        findPrevCommand.Received(1).Execute(Arg.Any<object?>());
        findNextCommand.Received(1).Execute(Arg.Any<object?>());
    }
    #endregion

    #region OnDialogOpened
    [Fact(DisplayName = "【正常系】OnDialogOpened:選択中の文字列が検索文字列に設定されること")]
    public void OnDialogOpened_ShouldUseSelectedText()
    {
        Document.SelectedText.Returns(new ReactiveProperty<string>("selected"));
        using var viewModel = new FindDialogViewModel(EditorService, SettingsService);

        viewModel.OnDialogOpened(new DialogParameters());

        Assert.Equal("selected", viewModel.SearchText.Value);
        Assert.Equal("selected", Document.SearchText.Value);
    }
    #endregion

    #region CancelCommand
    [Fact(DisplayName = "【正常系】CancelCommand:キャンセルでダイアログが閉じること")]
    public void CancelCommand_ShouldCloseWithCancel()
    {
        using var viewModel = new FindDialogViewModel(EditorService, SettingsService);
        IDialogResult? result = null;
        DialogCloseListenerTestHelper.AttachRequestClose(viewModel, r => result = r);

        viewModel.CancelCommand.Execute();

        Assert.Equal(ButtonResult.Cancel, result?.Result);
    }
    #endregion

    #region Dispose
    [Fact(DisplayName = "【正常系】Dispose:破棄後にViewModelの変更がDocumentに反映されないこと")]
    public void Dispose_ShouldStopViewToModelSubscriptions()
    {
        var viewModel = new FindDialogViewModel(EditorService, SettingsService);
        viewModel.SearchText.Value = "before";

        viewModel.Dispose();
        viewModel.SearchText.Value = "after";

        Assert.Equal("before", Document.SearchText.Value);
    }
    #endregion
}
